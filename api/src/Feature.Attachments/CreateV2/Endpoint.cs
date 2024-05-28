using System.Net;
using Authorization.Policies.Requirements;
using Feature.Attachments.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Attachments.CreateV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<AttachmentAggregate> repository,
    IFileStorageService fileStorageService,
    IReadRepository<MonitoringObserver> monitoringObserverRepository)
    : Endpoint<Request, Results<Ok<Result>, NotFound, BadRequest<ProblemDetails>, StatusCodeHttpResult>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/attachments:v2");
        DontAutoTag();
        Options(x => x.WithTags("attachments", "mobile"));
        AllowFileUploads();
        Summary(s =>
        {
            s.Summary = "Uploads an attachment for a specific polling station";
        });
    }

    public override async Task<Results<Ok<Result>, NotFound, BadRequest<ProblemDetails>, StatusCodeHttpResult>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }
        var monitoringObserverSpecification = new GetMonitoringObserverSpecification(req.ElectionRoundId, req.ObserverId);
        var monitoringObserver = await monitoringObserverRepository.FirstOrDefaultAsync(monitoringObserverSpecification, ct);

        if (monitoringObserver == null)
        {
            AddError(r => r.ObserverId, "Observer not found");
            return TypedResults.BadRequest(new ProblemDetails(ValidationFailures));
        }

        var uploadPath = $"elections/{req.ElectionRoundId}/polling-stations/{req.PollingStationId}/form/{req.FormId}/attachments";

        var attachment = new AttachmentAggregate(req.Id,
            req.ElectionRoundId,
            req.PollingStationId,
            monitoringObserver.Id,
            req.FormId,
            req.QuestionId,
            req.FileName,
            uploadPath,
            req.ContentType);

        var getPresignedUploadUrlResult = await fileStorageService.GetPresignedUploadLinkAsync(uploadPath, fileName: attachment.UploadedFileName, ct);

        if (getPresignedUploadUrlResult is PresignedUploadLinkResult.Failed)
        {
            return TypedResults.StatusCode((int)HttpStatusCode.InternalServerError);
        }

        await repository.AddAsync(attachment, ct);

        var result = getPresignedUploadUrlResult as PresignedUploadLinkResult.Ok;

        return TypedResults.Ok(new Result
        {
            Id = attachment.Id,
            PresignedUrl = result!.Url,
            UrlValidityInSeconds = result.UrlValidityInSeconds,
            FormId = attachment.FormId,
            PollingStationId = attachment.PollingStationId,
            QuestionId = attachment.QuestionId,
        });
    }
}
