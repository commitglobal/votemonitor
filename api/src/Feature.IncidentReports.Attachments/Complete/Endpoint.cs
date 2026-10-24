using Feature.IncidentReports.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.IncidentReports.Attachments.Complete;

public class Endpoint(
    IRepository<IncidentReportAttachmentAggregate> repository,
    IFileStorageService fileStorageService,
    IRecordAutomation recordAutomation)
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
        var isNew = !attachment.IsCompleted && !attachment.IsDeleted;
        attachment.Complete();
        await repository.UpdateAsync(attachment, ct);
        if (isNew)
        {
            await recordAutomation.RecordChangedByAuthorAsync(req.ElectionRoundId, ReportRecordType.IncidentReport,
                req.IncidentReportId, ct);
        }

        return TypedResults.NoContent();
    }
}
