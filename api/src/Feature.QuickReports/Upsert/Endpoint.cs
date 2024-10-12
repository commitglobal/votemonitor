using Authorization.Policies;
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
        Summary(s => { s.Summary = "Upserts a quick report"; });
        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<Results<Ok<QuickReportModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var quickReport =
            await repository.FirstOrDefaultAsync(new GetQuickReportByIdSpecification(req.ElectionRoundId, req.Id), ct);

        return quickReport is null
            ? await AddQuickReportAsync(req, ct)
            : await UpdateQuickReportAsync(req, quickReport, ct);
    }

    private async Task<Results<Ok<QuickReportModel>, NotFound>> AddQuickReportAsync(Request req,
        CancellationToken ct)
    {
        var monitoringObserverSpecification =
            new GetMonitoringObserverSpecification(req.ElectionRoundId, req.ObserverId);
        var monitoringObserver =
            await monitoringObserverRepository.FirstOrDefaultAsync(monitoringObserverSpecification, ct);

        if (monitoringObserver == null)
        {
            AddError(r => r.ObserverId, "Observer not found");
            return TypedResults.NotFound();
        }

        var quickReport = QuickReport.Create(req.Id, req.ElectionRoundId, monitoringObserver.Id, req.Title,
            req.Description, req.QuickReportLocationType, req.PollingStationId, req.PollingStationDetails, req.IncidentCategory);

        await repository.AddAsync(quickReport, ct);

        var attachments = await GetPresignedAttachments(req.ElectionRoundId, req.Id, ct);

        return TypedResults.Ok(QuickReportModel.FromEntity(quickReport, attachments));
    }

    private async Task<Results<Ok<QuickReportModel>, NotFound>> UpdateQuickReportAsync(Request req,
        QuickReport quickReport,
        CancellationToken ct)
    {
        quickReport.Update(req.Title, req.Description, req.QuickReportLocationType, req.PollingStationId,
            req.PollingStationDetails, req.IncidentCategory);
        await repository.UpdateAsync(quickReport, ct);

        var attachments = await GetPresignedAttachments(req.ElectionRoundId, req.Id, ct);

        return TypedResults.Ok(QuickReportModel.FromEntity(quickReport, attachments));
    }

    private async Task<QuickReportAttachmentModel[]> GetPresignedAttachments(Guid electionRoundId,
        Guid quickReportId,
        CancellationToken ct)
    {
        var quickReportAttachments = await attachmentsRepository.ListAsync(
            new ListQuickReportAttachmentsSpecification(electionRoundId, quickReportId), ct);

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
                };
            });
        var attachments = await Task.WhenAll(tasks);

        return attachments;
    }
}