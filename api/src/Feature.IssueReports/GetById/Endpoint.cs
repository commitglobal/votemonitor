using Authorization.Policies.Requirements;
using Feature.IssueReports.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Answer.Module.Mappers;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain;
using Vote.Monitor.Form.Module.Mappers;

namespace Feature.IssueReports.GetById;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    IFileStorageService fileStorageService) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/issue-reports/{issueReportId}");
        DontAutoTag();
        Options(x => x.WithTags("issue-reports"));
        Policies(PolicyNames.NgoAdminsOnly);

        Summary(s => { s.Summary = "Gets issue report by id including notes and attachments"; });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var issueReport = await context
            .IssueReports
            .Include(x => x.Attachments)
            .Include(x => x.Notes)
            .Include(x => x.PollingStation)
            .Where(x =>
                x.ElectionRoundId == req.ElectionRoundId
                && x.Form.MonitoringNgo.NgoId == req.NgoId
                && x.Id == req.IssueReportId)
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);

        if (issueReport == null)
        {
            return TypedResults.NotFound();
        }

        var form = await context
            .Forms
            .Where(x =>
                x.ElectionRoundId == req.ElectionRoundId
                && x.Id == issueReport.FormId)
            .FirstAsync(ct);

        var tasks = issueReport.Attachments
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
            IssueReportId = issueReport.Id,
            FormId = form.Id,
            FormCode = form.Code,
            FormName = form.Name,
            FormDefaultLanguage = form.DefaultLanguage,
            Answers = issueReport.Answers.Select(AnswerMapper.ToModel).ToArray(),
            Notes = issueReport.Notes.Select(NoteModel.FromEntity).ToArray(),
            Attachments = attachments,
            Questions = form.Questions.Select(QuestionsMapper.ToModel).ToArray(),

            TimeSubmitted = issueReport.LastModifiedOn ?? issueReport.CreatedOn,
            FollowUpStatus = issueReport.FollowUpStatus,

            LocationType = issueReport.LocationType,
            LocationDescription = issueReport.LocationDescription,

            PollingStationId = issueReport.PollingStation?.Level1,
            PollingStationLevel1 = issueReport.PollingStation?.Level1,
            PollingStationLevel2 = issueReport.PollingStation?.Level2,
            PollingStationLevel3 = issueReport.PollingStation?.Level3,
            PollingStationLevel4 = issueReport.PollingStation?.Level4,
            PollingStationLevel5 = issueReport.PollingStation?.Level5,
        };

        return TypedResults.Ok(response);
    }
}