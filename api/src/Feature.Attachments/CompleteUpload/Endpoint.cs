using Authorization.Policies.Requirements;
using Feature.Attachments.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.Attachments.CompleteUpload;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<AttachmentAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/attachments/{id}:complete");
        DontAutoTag();
        Options(x => x.WithTags("attachments", "mobile"));
        Summary(s =>
        {
            s.Summary = "Completes upload for an attachment";
        });
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetAttachmentByIdSpecification(req.ElectionRoundId, req.ObserverId, req.Id);
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
