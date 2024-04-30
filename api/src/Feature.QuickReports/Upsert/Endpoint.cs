using Authorization.Policies.Requirements;
using Feature.QuickReports.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Feature.QuickReports.Upsert;

public class Endpoint(
    IAuthorizationService authorizationService,
    IReadRepository<MonitoringObserver> monitoringObserverRepository,
    IRepository<QuickReport> repository,
    IReadRepository<QuickReportAttachment> attachmentsRepository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<QuickReportModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/quick-reports");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports", "mobile"));
        Summary(s =>
        {
            s.Summary = "Upserts a quick report";
        });
    }

    public override async Task<Results<Ok<QuickReportModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var quickReport = await repository.FirstOrDefaultAsync(new GetQuickReportByIdSpecification(req.ElectionRoundId, req.ObserverId, req.Id), ct);

        var quickReportAttachments = await attachmentsRepository.ListAsync(
            new ListQuickReportAttachmentsForUserSpecification(req.ElectionRoundId, req.ObserverId, req.Id), ct);

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
                    ElectionRoundId = attachment.ElectionRoundId,
                    QuickReportId = attachment.QuickReportId
                };
            });
        var attachments = await Task.WhenAll(tasks);

        return quickReport is null ? await AddQuickReportAsync(req, attachments, ct) : await UpdateQuickReportAsync(req, quickReport, attachments, ct);
    }

    private async Task<Results<Ok<QuickReportModel>, NotFound>> AddQuickReportAsync(Request req,
        QuickReportAttachmentModel[] attachments,
        CancellationToken ct)
    {
        var monitoringObserverSpecification = new GetMonitoringObserverSpecification(req.ElectionRoundId, req.ObserverId);
        var monitoringObserver = await monitoringObserverRepository.FirstOrDefaultAsync(monitoringObserverSpecification, ct);

        if (monitoringObserver == null)
        {
            AddError(r => r.ObserverId, "Observer not found");
            return TypedResults.NotFound();
        }

        var quickReport = QuickReport.Create(req.Id, req.ElectionRoundId, monitoringObserver.Id, req.Title, req.Description, req.QuickReportLocationType, req.PollingStationId, req.PollingStationDetails);

        await repository.AddAsync(quickReport, ct);

        return TypedResults.Ok(QuickReportModel.FromEntity(quickReport, attachments));
    }

    private async Task<Results<Ok<QuickReportModel>, NotFound>> UpdateQuickReportAsync(Request req,
        QuickReport quickReport,
        QuickReportAttachmentModel[] attachments,
        CancellationToken ct)
    {
        quickReport.Update(req.Title, req.Description, req.QuickReportLocationType, req.PollingStationId, req.PollingStationDetails);
        await repository.UpdateAsync(quickReport, ct);

        return TypedResults.Ok(QuickReportModel.FromEntity(quickReport, attachments));
    }
}
