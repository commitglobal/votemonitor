using Authorization.Policies.Requirements;
using Feature.Attachments.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.Attachments.AbortUpload;

public class Endpoint(IAuthorizationService authorizationService,
    IRepository<AttachmentAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/attachments/{id}:abort");
        DontAutoTag();
        Options(x => x.WithTags("attachments", "mobile"));
        AllowFileUploads();
        Summary(s =>
        {
            s.Summary = "Aborts the upload and marks the attachment as deleted";
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

        attachment.Delete();
        await repository.UpdateAsync(attachment, ct);

        await fileStorageService.AbortUploadAsync(req.UploadId, attachment.FilePath, attachment.UploadedFileName, ct);

        return TypedResults.NoContent();
    }
}
