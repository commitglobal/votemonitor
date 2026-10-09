using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.DataExport.Start;
using Job.Contracts;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;

namespace Feature.DataExport.StartV2;

public class Endpoint(
    ICurrentUserRoleProvider userRoleProvider,
    ICurrentUserProvider userProvider,
    IJobService jobService,
    IAuthorizationService authorizationService,
    IRepository<ExportedData> repository,
    ITimeProvider timeProvider) : Endpoint<Request, Results<Ok<Response>, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Post("/api/exported-data:v2");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("exported-data"));

        Summary(s => { s.Summary = "Enqueues a job to export data and returns job id to poll for results (v2)"; });
        Policies(PolicyNames.NotObservers);
    }

    public override async Task<Results<Ok<Response>, NotFound, ProblemDetails>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        if (userRoleProvider.IsNgoAdmin())
        {
            var result =
                await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
            if (!result.Succeeded)
            {
                return TypedResults.NotFound();
            }
        }

        if (req.ExportedDataType == ExportedDataType.FormSubmissions
            || req.ExportedDataType == ExportedDataType.QuickReports
            || req.ExportedDataType == ExportedDataType.CitizenReports
            || req.ExportedDataType == ExportedDataType.IncidentReports
            || req.ExportedDataType == ExportedDataType.FormSubmissionsSimplified)
        {
            if (!userRoleProvider.IsNgoAdmin())
            {
                AddError(x => x.ExportedDataType, "Only ngo admins can export this type of data");
                return new ProblemDetails(ValidationFailures);
            }

            if (!userProvider.GetNgoId().HasValue)
            {
                AddError("ngoId", "Ngo Id is required");
                return new ProblemDetails(ValidationFailures);
            }
        }

        if (req.ExportedDataType == ExportedDataType.PollingStationInformation)
        {
            if (!userRoleProvider.IsPlatformAdmin())
            {
                AddError(x => x.ExportedDataType, "Only platform admins can export this type of data");
                return new ProblemDetails(ValidationFailures);
            }
        }

        var exportedData = CreateExportedData(req, userProvider.GetNgoId());

        await repository.AddAsync(exportedData, ct);

        if (req.ExportedDataType == ExportedDataType.FormSubmissions)
        {
            jobService.EnqueueExportFormSubmissions(req.ElectionRoundId, userProvider.GetNgoId()!.Value,
                exportedData.Id);
        }

        if (req.ExportedDataType == ExportedDataType.QuickReports)
        {
            jobService.EnqueueExportQuickReports(req.ElectionRoundId, userProvider.GetNgoId()!.Value, exportedData.Id);
        }

        if (req.ExportedDataType == ExportedDataType.CitizenReports)
        {
            jobService.EnqueueExportCitizenReports(req.ElectionRoundId, userProvider.GetNgoId()!.Value, exportedData.Id);
        }

        if (req.ExportedDataType == ExportedDataType.IncidentReports)
        {
            jobService.EnqueueExportIncidentReports(req.ElectionRoundId, userProvider.GetNgoId()!.Value,
                exportedData.Id);
        }

        if (req.ExportedDataType == ExportedDataType.PollingStations)
        {
            jobService.EnqueueExportPollingStations(req.ElectionRoundId, exportedData.Id);
        }

        if (req.ExportedDataType == ExportedDataType.Locations)
        {
            jobService.EnqueueExportLocations(req.ElectionRoundId, exportedData.Id);
        }

        if (req.ExportedDataType == ExportedDataType.FormSubmissionsSimplified)
        {
            jobService.EnqueueExportFormSubmissionsSimplified(req.ElectionRoundId, userProvider.GetNgoId()!.Value,
                exportedData.Id);
        }

        if (req.ExportedDataType == ExportedDataType.PollingStationInformation)
        {
            jobService.EnqueueExportPollingStationInformation(req.ElectionRoundId, exportedData.Id);
        }

        return TypedResults.Ok(new Response { ExportedDataId = exportedData.Id, EnqueuedAt = timeProvider.UtcNow });
    }

    private ExportedData CreateExportedData(Request req, Guid? ngoId)
    {
        if (req.ExportedDataType == ExportedDataType.FormSubmissions)
        {
            return ExportedData.CreateForFormSubmissions(req.UserId,
                timeProvider.UtcNow,
                new ExportFormSubmissionsFilters { NgoId = ngoId!.Value, DataSource = req.DataSource },
                req.FilterConditions);
        }

        if (req.ExportedDataType == ExportedDataType.FormSubmissionsSimplified)
        {
            return ExportedData.CreateForFormSubmissionsSimplified(req.UserId,
                timeProvider.UtcNow,
                new ExportFormSubmissionsFilters { NgoId = ngoId!.Value, DataSource = req.DataSource },
                req.FilterConditions);
        }

        if (req.ExportedDataType == ExportedDataType.QuickReports)
        {
            return ExportedData.CreateForQuickReports(req.UserId,
                timeProvider.UtcNow,
                new ExportQuickReportsFilters { NgoId = ngoId!.Value, DataSource = req.DataSource },
                req.FilterConditions);
        }

        if (req.ExportedDataType == ExportedDataType.CitizenReports)
        {
            return ExportedData.CreateForCitizenReports(req.UserId,
                timeProvider.UtcNow,
                new ExportCitizenReportsFilers { NgoId = ngoId!.Value },
                req.FilterConditions);
        }

        if (req.ExportedDataType == ExportedDataType.IncidentReports)
        {
            return ExportedData.CreateForIncidentReports(req.UserId,
                timeProvider.UtcNow,
                new ExportIncidentReportsFilters { NgoId = ngoId!.Value, DataSource = req.DataSource },
                req.FilterConditions);
        }

        return ExportedData.Create(req.UserId, req.ExportedDataType, timeProvider.UtcNow);
    }
}
