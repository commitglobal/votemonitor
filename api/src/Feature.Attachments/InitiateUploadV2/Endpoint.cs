using Authorization.Policies.Requirements;
using Feature.Attachments.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Attachments.InitiateUploadV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<AttachmentAggregate> repository,
    IFileStorageService fileStorageService,
    IReadRepository<MonitoringObserver> monitoringObserverRepository,
    ITimeProvider timeProvider)
    : Endpoint<Request, Results<Ok<Response>, NotFound, Conflict>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}/attachments:init");
        DontAutoTag();
        Options(x => x.WithTags("attachments", "mobile"));
        Summary(s =>
        {
            s.Summary =
                "Creates an attachment for a specific polling station and gets back details for uploading it in the file storage";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound, Conflict>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetAttachmentByIdSpecification(req.ElectionRoundId, req.ObserverId, req.Id);
        var duplicatedAttachmentExist = await repository.AnyAsync(specification, ct);
        if (duplicatedAttachmentExist)
        {
            return TypedResults.Conflict();
        }

        var monitoringObserverSpecification =
            new GetMonitoringObserverIdSpecification(req.ElectionRoundId, req.ObserverId);
        var monitoringObserverId =
            await monitoringObserverRepository.FirstOrDefaultAsync(monitoringObserverSpecification, ct);

        var uploadPath =
            $"elections/{req.ElectionRoundId}/{req.SubmissionId}";
       
        var attachment = AttachmentAggregate.CreateV2(req.Id,
            req.SubmissionId,
            monitoringObserverId,
            req.QuestionId,
            req.FileName,
            uploadPath,
            req.ContentType,
            req.LastUpdatedAt);

        var uploadResult = await fileStorageService.CreateMultipartUploadAsync(uploadPath,
            fileName: attachment.UploadedFileName,
            contentType: req.ContentType,
            numberOfUploadParts: req.NumberOfUploadParts,
            ct: ct);

        await repository.AddAsync(attachment, ct);

        return TypedResults.Ok(new Response
        {
            UploadId = uploadResult.UploadId, UploadUrls = uploadResult.PresignedUrls
        });
    }
}
