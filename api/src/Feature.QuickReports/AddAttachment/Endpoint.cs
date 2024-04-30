using System.Net;
using Authorization.Policies.Requirements;
using Feature.QuickReports.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Feature.QuickReports.AddAttachment;

public class Endpoint(
    IAuthorizationService authorizationService,
    IRepository<QuickReportAttachment> repository,
    IFileStorageService fileStorageService,
    IReadRepository<MonitoringObserver> monitoringObserverRepository)
    : Endpoint<Request, Results<Ok<QuickReportAttachmentModel>, NotFound, BadRequest<ProblemDetails>, StatusCodeHttpResult>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/quick-reports/{quickReportId}/attachments");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports", "mobile"));
        AllowFileUploads();
        Summary(s =>
        {
            s.Summary = "Uploads an attachment for a quick report";
        });
    }

    public override async Task<Results<Ok<QuickReportAttachmentModel>, NotFound, BadRequest<ProblemDetails>, StatusCodeHttpResult>> ExecuteAsync(Request req, CancellationToken ct)
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

        var uploadPath = $"elections/{req.ElectionRoundId}/quick-reports/{req.QuickReportId}";

        var attachment = QuickReportAttachment.Create(req.Id,
            req.ElectionRoundId,
            monitoringObserver.Id,
            req.QuickReportId,
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

        await repository.AddAsync(attachment, ct);

        var result = uploadResult as UploadFileResult.Ok;

        return TypedResults.Ok(new QuickReportAttachmentModel()
        {
            Id = attachment.Id,
            FileName = attachment.FileName,
            PresignedUrl = result!.Url,
            MimeType = attachment.MimeType,
            UrlValidityInSeconds = result.UrlValidityInSeconds,
            ElectionRoundId = attachment.ElectionRoundId,
            QuickReportId = attachment.QuickReportId,
        });
    }
}
