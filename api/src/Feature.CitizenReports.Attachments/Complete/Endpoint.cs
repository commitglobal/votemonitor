using Feature.CitizenReports.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.CitizenReports.Attachments.Complete;

public class Endpoint(
    IRepository<CitizenReportAttachmentAggregate> repository,
    IFileStorageService fileStorageService,
    IRecordAutomation recordAutomation)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/citizen-reports/{citizenReportId}/attachments/{id}:complete");
        DontAutoTag();
        AllowAnonymous();
        Options(x => x.WithTags("citizen-report-attachments", "public"));
        Summary(s =>
        {
            s.Summary = "Completes upload for an attachment";
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

        await fileStorageService.CompleteUploadAsync(req.UploadId, attachment.FilePath, attachment.UploadedFileName, req.Etags, ct);
        var isNew = !attachment.IsCompleted && !attachment.IsDeleted;
        attachment.Complete();
        await repository.UpdateAsync(attachment, ct);
        if (isNew)
        {
            await recordAutomation.RecordChangedByAuthorAsync(req.ElectionRoundId, ReportRecordType.CitizenReport,
                req.CitizenReportId, ct);
        }

        return TypedResults.NoContent();
    }
}
