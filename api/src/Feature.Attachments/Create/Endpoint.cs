using System.Net;
using Authorization.Policies.Requirements;
using Feature.Attachments.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Attachments.Create;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<AttachmentAggregate> repository,
    IFileStorageService fileStorageService,
    IReadRepository<MonitoringObserver> monitoringObserverRepository)
    : Endpoint<Request, Results<Ok<AttachmentModel>, NotFound, BadRequest<ProblemDetails>, StatusCodeHttpResult>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/attachments");
        DontAutoTag();
        Options(x => x.WithTags("attachments", "mobile"));
        AllowFileUploads();
        Summary(s =>
        {
            s.Summary = "Uploads an attachment for a specific polling station";
        });
    }

    public override async Task<Results<Ok<AttachmentModel>, NotFound, BadRequest<ProblemDetails>, StatusCodeHttpResult>> ExecuteAsync(Request req, CancellationToken ct)
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

        var attachment = AttachmentAggregate.Create(req.Id,
            req.ElectionRoundId,
            req.PollingStationId,
            monitoringObserver.Id,
            req.FormId,
            req.QuestionId,
            req.Attachment.FileName,
            uploadPath,
            req.Attachment.ContentType);

        var uploadResult = await fileStorageService.UploadFileAsync(uploadPath,
            fileName: attachment.UploadedFileName,
            req.Attachment.OpenReadStream(),
            ct);

        if (uploadResult is UploadFileResult.Failed)
        {
            return TypedResults.StatusCode((int)HttpStatusCode.InternalServerError);
        }

        attachment.Complete();

        await repository.AddAsync(attachment, ct);

        var result = uploadResult as UploadFileResult.Ok;

        return TypedResults.Ok(new AttachmentModel
        {
            FileName = attachment.FileName,
            PresignedUrl = result!.Url,
            MimeType = attachment.MimeType,
            UrlValidityInSeconds = result.UrlValidityInSeconds,
            Id = attachment.Id,
            ElectionRoundId = attachment.ElectionRoundId,
            PollingStationId = attachment.PollingStationId,
            FormId = attachment.FormId,
            QuestionId = attachment.QuestionId,
        });
    }
}
