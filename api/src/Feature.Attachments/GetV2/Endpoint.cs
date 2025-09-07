using Authorization.Policies.Requirements;
using Feature.Attachments.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.Attachments.GetV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReadRepository<AttachmentAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<AttachmentModelV2>, BadRequest<ProblemDetails>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("attachments", "mobile"));
        Summary(s => {
            s.Summary = "Gets an attachment";
            s.Description = "Gets an attachment with freshly generated presigned url";
        });
    }

    public override async Task<Results<Ok<AttachmentModelV2>, BadRequest<ProblemDetails>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetAttachmentByIdSpecification(req.ElectionRoundId, req.ObserverId, req.Id);
        var attachment = await repository.FirstOrDefaultAsync(specification, ct);
        
        if (attachment is null)
        {
            return TypedResults.NotFound();
        }

        var presignedUrl = await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);

        return TypedResults.Ok(new AttachmentModelV2
        {
            Id = attachment.Id,
            ElectionRoundId = req.ElectionRoundId,
            FileName = attachment.FileName,
            PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
            MimeType = attachment.MimeType,
            UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
            SubmissionId = attachment.SubmissionId,
            QuestionId = attachment.QuestionId
        });
    }
}
