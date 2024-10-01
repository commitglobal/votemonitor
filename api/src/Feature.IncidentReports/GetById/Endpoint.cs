using Authorization.Policies.Requirements;
using Feature.IncidentReports.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Answer.Module.Mappers;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain;
using Vote.Monitor.Form.Module.Mappers;

namespace Feature.IncidentReports.GetById;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    IFileStorageService fileStorageService) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/incident-reports/{incidentReportId}");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports"));
        Policies(PolicyNames.NgoAdminsOnly);

        Summary(s => { s.Summary = "Gets incident report by id including notes and attachments"; });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var incidentReport = await context
            .IncidentReports
            .Include(x => x.Attachments)
            .Include(x => x.Notes)
            .Include(x => x.PollingStation)
            .Include(x => x.MonitoringObserver)
            .ThenInclude(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .Where(x =>
                x.ElectionRoundId == req.ElectionRoundId
                && x.Form.MonitoringNgo.NgoId == req.NgoId
                && x.Id == req.IncidentReportId)
            .Select(incidentReport => new
            {
                Id = incidentReport.Id,
                FormId = incidentReport.Form.Id,
                FormCode = incidentReport.Form.Code,
                FormName = incidentReport.Form.Name,
                FormDefaultLanguage = incidentReport.Form.DefaultLanguage,
                Answers = incidentReport.Answers,
                Notes = incidentReport.Notes,
                Attachments = incidentReport.Attachments,
                Questions = incidentReport.Form.Questions,
                MonitoringObserverId = incidentReport.MonitoringObserverId,
                ObserverName = incidentReport.MonitoringObserver.Observer.ApplicationUser.LastName + " " +
                               incidentReport.MonitoringObserver.Observer.ApplicationUser.LastName,
                TimeSubmitted = incidentReport.LastModifiedOn ?? incidentReport.CreatedOn,
                FollowUpStatus = incidentReport.FollowUpStatus,

                LocationType = incidentReport.LocationType,
                LocationDescription = incidentReport.LocationDescription,

                PollingStation = incidentReport.PollingStation,
            })
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);

        if (incidentReport == null)
        {
            return TypedResults.NotFound();
        }

        var form = await context
            .Forms
            .Where(x =>
                x.ElectionRoundId == req.ElectionRoundId
                && x.Id == incidentReport.FormId)
            .FirstAsync(ct);

        var tasks = incidentReport.Attachments
            .Select(AttachmentModel.FromEntity)
            .Select(async attachment =>
            {
                var presignedUrl = await fileStorageService.GetPresignedUrlAsync(
                    attachment.FilePath,
                    attachment.UploadedFileName);

                return attachment with
                {
                    PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
                    UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
                };
            }).ToArray();

        var attachments = await Task.WhenAll(tasks);
        var response = new Response
        {
            IncidentReportId = incidentReport.Id,
            FormId = form.Id,
            FormCode = form.Code,
            FormName = form.Name,
            FormDefaultLanguage = form.DefaultLanguage,
            Answers = incidentReport.Answers.Select(AnswerMapper.ToModel).ToArray(),
            Notes = incidentReport.Notes.Select(NoteModel.FromEntity).ToArray(),
            Attachments = attachments,
            Questions = form.Questions.Select(QuestionsMapper.ToModel).ToArray(),

            MonitoringObserverId = incidentReport.MonitoringObserverId,
            ObserverName = incidentReport.ObserverName,
            TimeSubmitted = incidentReport.TimeSubmitted,
            FollowUpStatus = incidentReport.FollowUpStatus,

            LocationType = incidentReport.LocationType,
            LocationDescription = incidentReport.LocationDescription,

            PollingStationId = incidentReport.PollingStation?.Level1,
            PollingStationLevel1 = incidentReport.PollingStation?.Level1,
            PollingStationLevel2 = incidentReport.PollingStation?.Level2,
            PollingStationLevel3 = incidentReport.PollingStation?.Level3,
            PollingStationLevel4 = incidentReport.PollingStation?.Level4,
            PollingStationLevel5 = incidentReport.PollingStation?.Level5,
            PollingStationNumber = incidentReport.PollingStation?.Number,
        };

        return TypedResults.Ok(response);
    }
}