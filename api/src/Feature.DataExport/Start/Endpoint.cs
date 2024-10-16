using Authorization.Policies;
using Authorization.Policies.Requirements;
using Job.Contracts;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;

namespace Feature.DataExport.Start;

public class Endpoint(
    IJobService jobService,
    IAuthorizationService authorizationService,
    IRepository<ExportedData> repository,
    ITimeProvider timeProvider) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/exported-data");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("exported-data"));

        Summary(s => { s.Summary = "Enqueues a job to export data and returns job id to poll for results"; });
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var result =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!result.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var exportedData = CreateExportedData(req);

        await repository.AddAsync(exportedData, ct);

        if (req.ExportedDataType == ExportedDataType.FormSubmissions)
        {
            jobService.EnqueueExportFormSubmissions(req.ElectionRoundId, req.NgoId, exportedData.Id);
        }

        if (req.ExportedDataType == ExportedDataType.QuickReports)
        {
            jobService.EnqueueExportQuickReports(req.ElectionRoundId, req.NgoId, exportedData.Id);
        }

        if (req.ExportedDataType == ExportedDataType.PollingStations)
        {
            jobService.EnqueueExportPollingStations(req.ElectionRoundId, exportedData.Id);
        }

        if (req.ExportedDataType == ExportedDataType.CitizenReports)
        {
            jobService.EnqueueExportCitizenReports(req.ElectionRoundId, req.NgoId, exportedData.Id);
        }

        if (req.ExportedDataType == ExportedDataType.Locations)
        {
            jobService.EnqueueExportLocations(req.ElectionRoundId, exportedData.Id);
        }

        if (req.ExportedDataType == ExportedDataType.IncidentReports)
        {
            jobService.EnqueueExportIncidentReports(req.ElectionRoundId, req.NgoId, exportedData.Id);
        }

        return TypedResults.Ok(new Response
        {
            ExportedDataId = exportedData.Id,
            EnqueuedAt = timeProvider.UtcNow
        });
    }

    private ExportedData CreateExportedData(Request req)
    {
        if (req.ExportedDataType == ExportedDataType.FormSubmissions)
        {
            return ExportedData.CreateForFormSubmissions(req.ElectionRoundId, req.ExportedDataType, timeProvider.UtcNow,
                req.FormSubmissionsFilters?.ToFilter());
        }

        if (req.ExportedDataType == ExportedDataType.QuickReports)
        {
            return ExportedData.CreateForQuickReports(req.ElectionRoundId, req.ExportedDataType, timeProvider.UtcNow,
                req.QuickReportsFilters?.ToFilter());
        }

        if (req.ExportedDataType == ExportedDataType.CitizenReports)
        {
            return ExportedData.CreateForCitizenReports(req.ElectionRoundId, req.ExportedDataType, timeProvider.UtcNow,
                req.CitizenReportsFilters?.ToFilter());
        }
        
        if (req.ExportedDataType == ExportedDataType.IncidentReports)
        {
            return ExportedData.CreateForIncidentReports(req.ElectionRoundId, req.ExportedDataType, timeProvider.UtcNow,
                req.IncidentReportsFilters?.ToFilter());
        }

        return ExportedData.Create(req.ElectionRoundId, req.ExportedDataType, timeProvider.UtcNow);
    }
}