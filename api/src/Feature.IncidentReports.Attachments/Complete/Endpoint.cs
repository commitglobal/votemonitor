using Feature.IncidentReports.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.IncidentReports.Attachments.Complete;

public class Endpoint(
    IRepository<IncidentReportAttachmentAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/incident-reports/{incidentReportId}/attachments/{id}:complete");
        DontAutoTag();
        Options(x => x.WithTags("incident-report-attachments"));
        Summary(s =>
        {
            s.Summary = "Completes upload for an attachment";
        });
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetAttachmentByIdSpecification(req.ElectionRoundId, req.IncidentReportId, req.Id);
        var attachment = await repository.FirstOrDefaultAsync(specification, ct);

        if (attachment == null)
        {
            return TypedResults.NotFound();
        }

        await fileStorageService.CompleteUploadAsync(req.UploadId, attachment.FilePath, attachment.UploadedFileName, req.Etags, ct);
        attachment.Complete();
        await repository.UpdateAsync(attachment, ct);

        return TypedResults.NoContent();
    }
}
