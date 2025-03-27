using Authorization.Policies.Requirements;
using Feature.QuickReports.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Feature.QuickReports.CompleteUpload;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<QuickReportAttachment> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/quick-reports/{quickReportId}/attachments/{id}:complete");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports", "mobile"));
        Summary(s =>
        {
            s.Summary = "Completes upload for a quick report attachment";
        });
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetQuickReportAttachmentByIdSpecification(req.ElectionRoundId, req.ObserverId, req.QuickReportId, req.Id);
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
