using Feature.CitizenReports.Models;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Answer.Module.Mappers;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain;
using Vote.Monitor.Form.Module.Mappers;

namespace Feature.CitizenReports.GetById;

public class Endpoint(
    VoteMonitorContext context,
    IFileStorageService fileStorageService) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-reports/{reportId}");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.NgoAdminsOnly);

        Summary(s => { s.Summary = "Gets citizen report by id including notes and attachments"; });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var citizenReport = await context
            .CitizenReports
            .Include(x => x.Attachments)
            .Include(x => x.Notes)
            .Where(x =>
                x.ElectionRoundId == req.ElectionRoundId
                && x.Form.MonitoringNgo.NgoId == req.NgoId
                && x.Id == req.CitizenReportId)
            .FirstOrDefaultAsync(ct);

        if (citizenReport == null)
        {
            return TypedResults.NotFound();
        }

        var form = await context
            .Forms
            .Where(x =>
                x.ElectionRoundId == req.ElectionRoundId
                && x.Id == citizenReport.FormId)
            .FirstAsync(ct);

        var tasks = citizenReport.Attachments
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
            ReportId = citizenReport.Id,
            Answers = citizenReport.Answers.Select(AnswerMapper.ToModel).ToArray(),
            Notes = citizenReport.Notes.Select(NoteModel.FromEntity).ToArray(),
            Attachments = attachments,
            FormId = form.Id,
            Questions = form.Questions.Select(QuestionsMapper.ToModel).ToArray(),

            Email = citizenReport.Email,
            ContactInformation = citizenReport.ContactInformation,

            TimeSubmitted = citizenReport.LastModifiedOn ?? citizenReport.CreatedOn,
            FollowUpStatus = citizenReport.FollowUpStatus
        };

        return TypedResults.Ok(response);
    }
}