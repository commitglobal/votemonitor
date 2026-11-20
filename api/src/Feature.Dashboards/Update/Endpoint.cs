using Feature.Dashboards.Models;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain;

namespace Feature.Dashboards.Update;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context) : Endpoint<Request, Results<Ok<DashboardModel>, NotFound, Conflict<VersionConflict<DashboardModel>>>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/dashboards/{dashboardId}");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("dashboards"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Replaces a dashboard's name, description and charts";
            s.Description = "NGO admins, also in archived rounds. Moving or resizing a chart saves the whole dashboard. " +
                            "Another NGO's dashboard is 404. With a version older than the current one: 409 with the current " +
                            "dashboard, nothing saved.";
        });
    }

    public override async Task<Results<Ok<DashboardModel>, NotFound, Conflict<VersionConflict<DashboardModel>>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var monitoringNgoId = await context.ResolveMonitoringNgoIdAsync(authorizationService, User, req.ElectionRoundId,
            req.NgoId, ct);
        var dashboard = await context.Dashboards.FirstOrDefaultAsync(x => x.Id == req.DashboardId
                                                                          && x.ElectionRoundId == req.ElectionRoundId
                                                                          && x.MonitoringNgoId == monitoringNgoId, ct);
        if (monitoringNgoId is null || dashboard is null)
        {
            return TypedResults.NotFound();
        }

        if (RecordVersions.IsStale(req.Version, dashboard.Version))
        {
            return await ConflictAsync(req, ct);
        }

        dashboard.Update(req.Name.Trim(),
            DashboardCharts.Description(req.Description),
            DashboardCharts.ToDocument(req.Charts));
        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Saved by someone else between reading and writing it.
            context.ChangeTracker.Clear();
            return await ConflictAsync(req, ct);
        }

        var model = (await context.ToModelsAsync([dashboard], true, ct)).Single();
        return TypedResults.Ok(model);
    }

    private async Task<Results<Ok<DashboardModel>, NotFound, Conflict<VersionConflict<DashboardModel>>>> ConflictAsync(Request req, CancellationToken ct)
    {
        var current = await context.Dashboards
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == req.DashboardId && x.ElectionRoundId == req.ElectionRoundId, ct);
        if (current is null)
        {
            return TypedResults.NotFound();
        }

        var model = (await context.ToModelsAsync([current], true, ct)).Single();
        return TypedResults.Conflict(VersionConflict<DashboardModel>.Of(model));
    }
}
