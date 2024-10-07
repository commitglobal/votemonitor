using Feature.CitizenReports.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.CitizenReports.Attachments.AbortUpload;

public class Endpoint(IRepository<CitizenReportAttachmentAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/citizen-reports/{citizenReportId}/attachments/{id}:abort");
        DontAutoTag();
        AllowAnonymous();
        Options(x => x.WithTags("citizen-report-attachments", "public"));
        Summary(s =>
        {
            s.Summary = "Aborts the upload and marks the attachment as deleted";
        });
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetAttachmentByIdSpecification(req.ElectionRoundId, req.CitizenReportId, req.Id);
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
