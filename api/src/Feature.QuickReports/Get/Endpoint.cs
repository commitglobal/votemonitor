using Authorization.Policies.Requirements;
using Feature.QuickReports.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Feature.QuickReports.Get;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReadRepository<QuickReport> quickReportRepository,
    IReadRepository<QuickReportAttachment> quickReportAttachmentRepository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<QuickReportDetailedModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/quick-reports/{id}");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports", "mobile"));
        Summary(s =>
        {
            s.Summary = "Gets details regarding a quick report";
            s.Description = "Gets a quick report with it's attachments and refreshed presigned urls";
        });
    }

    public override async Task<Results<Ok<QuickReportDetailedModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var quickReport = await quickReportRepository.FirstOrDefaultAsync(new GetQuickReportByIdSpecification(req.ElectionRoundId, req.Id), ct);

        if (quickReport is null)
        {
            return TypedResults.NotFound();
        }

        var quickReportAttachments = await quickReportAttachmentRepository.ListAsync(new ListQuickReportAttachmentsSpecification(req.ElectionRoundId, quickReport.Id), ct);

        var tasks = quickReportAttachments
            .Select(async attachment =>
            {
                var presignedUrl = await fileStorageService.GetPresignedUrlAsync(
                    attachment.FilePath,
                    attachment.UploadedFileName,
                    ct);

                return new QuickReportAttachmentModel
                {
                    FileName = attachment.FileName,
                    PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
                    MimeType = attachment.MimeType,
                    UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
                    Id = attachment.Id,
                    QuickReportId = attachment.QuickReportId
                };
            });

        var attachments = await Task.WhenAll(tasks);

        return TypedResults.Ok(QuickReportDetailedModel.FromEntity(quickReport, attachments));
    }
}
