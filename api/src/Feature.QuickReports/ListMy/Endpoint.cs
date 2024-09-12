using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.QuickReports.Models;
using Feature.QuickReports.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Feature.QuickReports.ListMy;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReadRepository<QuickReport> quickReportRepository,
    IReadRepository<QuickReportAttachment> quickReportAttachmentRepository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<List<QuickReportModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/quick-reports:my");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports", "mobile"));
        Summary(s =>
        {
            s.Summary = "Gets all quick-reports an observer has uploaded for an election round";
            s.Description = "All attachments will have freshly generated presigned urls";
        });

        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<Results<Ok<List<QuickReportModel>>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var quickReports = await quickReportRepository.ListAsync(new ListObserverQuickReportsSpecification(req.ElectionRoundId, req.ObserverId), ct);
        var quickReportIds = quickReports.Select(x => x.Id).ToArray();

        var quickReportAttachments = await quickReportAttachmentRepository.ListAsync(new ListQuickReportAttachmentsSpecification(req.ElectionRoundId, quickReportIds), ct);

        var tasks = quickReportAttachments
            .Select(async attachment =>
            {
                var presignedUrl = await fileStorageService.GetPresignedUrlAsync(
                    attachment.FilePath,
                    attachment.UploadedFileName);

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
        var attachmentsPerQuickReport = attachments
            .GroupBy(x => x.QuickReportId)
            .ToDictionary(x => x.Key, v => v.ToList());

        var result = quickReports
            .Select(quickReport => QuickReportModel.FromEntity(quickReport, attachmentsPerQuickReport.GetValueOrDefault(quickReport.Id, [])))
            .ToList();

        return TypedResults.Ok(result);
    }
}
