using Feature.IssueReports.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.IssueReports.Attachments.AbortUpload;

public class Endpoint(IRepository<IssueReportAttachmentAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/issue-report-attachments/{id}:abort");
        DontAutoTag();
        Options(x => x.WithTags("issue-report-attachments"));
        Summary(s =>
        {
            s.Summary = "Aborts the upload and marks the attachment as deleted";
        });
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetAttachmentByIdSpecification(req.ElectionRoundId, req.IssueReportId, req.Id);
        var attachment = await repository.FirstOrDefaultAsync(specification, ct);

        if (attachment == null)
        {
            return TypedResults.NotFound();
        }

        attachment.Delete();
        await repository.UpdateAsync(attachment, ct);

        await fileStorageService.AbortUploadAsync(req.UploadId, attachment.FilePath, attachment.UploadedFileName, ct);

        return TypedResults.NoContent();
    }
}
