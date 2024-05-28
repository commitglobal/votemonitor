using Authorization.Policies.Requirements;
using Feature.Attachments.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Attachments.InitiateUpload;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<AttachmentAggregate> repository,
    IFileStorageService fileStorageService,
    IReadRepository<MonitoringObserver> monitoringObserverRepository)
    : Endpoint<Request, Results<Ok<Response>, NotFound, BadRequest<ProblemDetails>, StatusCodeHttpResult>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/attachments");
        DontAutoTag();
        Options(x => x.WithTags("attachments", "mobile"));
        AllowFileUploads();
        Summary(s =>
        {
            s.Summary = "Creates an attachment for a specific polling station and gets back details for uploading it in the file storage";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound, BadRequest<ProblemDetails>, StatusCodeHttpResult>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var monitoringObserverSpecification = new GetMonitoringObserverSpecification(req.ElectionRoundId, req.ObserverId);
        var monitoringObserver = await monitoringObserverRepository.FirstOrDefaultAsync(monitoringObserverSpecification, ct);

        var uploadPath = $"elections/{req.ElectionRoundId}/polling-stations/{req.PollingStationId}/form/{req.FormId}/attachments";
        var attachment = new AttachmentAggregate(req.Id,
            req.ElectionRoundId,
            req.PollingStationId,
            monitoringObserver!.Id,
            req.FormId,
            req.QuestionId,
            req.FileName,
            uploadPath,
            req.ContentType);

        var uploadResult = await fileStorageService.CreateMultipartUploadAsync(uploadPath,
            fileName: attachment.UploadedFileName,
            contentType: req.ContentType,
            numberOfUploadParts: req.NumberOfUploadParts,
            ct: ct);

        await repository.AddAsync(attachment, ct);

        return TypedResults.Ok(new Response
        {
            UploadId = uploadResult.UploadId,
            UploadUrls = uploadResult.PresignedUrls
        });
    }
}
