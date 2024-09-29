using Feature.IssueReports.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.IssueReports.Attachments.Get;

public class Endpoint(
    IReadRepository<IssueReportAttachmentAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<IssueReportAttachmentModel>, BadRequest<ProblemDetails>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/issue-report-attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("issue-report-attachments"));
        Summary(s =>
        {
            s.Summary = "Gets an attachment";
            s.Description = "Gets an attachment with freshly generated presigned url";
        });
    }

    public override async Task<Results<Ok<IssueReportAttachmentModel>, BadRequest<ProblemDetails>, NotFound>>
        ExecuteAsync(Request req, CancellationToken ct)
    {
        var attachment =
            await repository.FirstOrDefaultAsync(
                new GetAttachmentByIdSpecification(req.ElectionRoundId, req.IssueReportId, req.Id), ct);
        if (attachment is null)
        {
            return TypedResults.NotFound();
        }

        var presignedUrl =
            await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);

        return TypedResults.Ok(new IssueReportAttachmentModel
        {
            FileName = attachment.FileName,
            PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
            MimeType = attachment.MimeType,
            UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
            Id = attachment.Id,
            ElectionRoundId = attachment.ElectionRoundId,
            IssueReportId = attachment.IssueReportId,
            FormId = attachment.FormId,
            QuestionId = attachment.QuestionId,
        });
    }
}