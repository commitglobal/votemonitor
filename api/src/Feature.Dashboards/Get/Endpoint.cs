using Feature.Dashboards.Models;

namespace Feature.Dashboards.Get;

public class Endpoint(
    IAuthorizationService authorizationService,
    ICurrentUserRoleProvider roleProvider,
    VoteMonitorContext context) : Endpoint<Request, Results<Ok<DashboardModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/dashboards/{dashboardId}");
        DontAutoTag();
        Options(x => x.WithTags("dashboards"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Gets one of the caller's NGO's dashboards";
            s.Description = "NGO admins and staff, also in archived rounds. Another NGO's dashboard is 404.";
        });
    }

    public override async Task<Results<Ok<DashboardModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var monitoringNgoId = await context.ResolveMonitoringNgoIdAsync(authorizationService, User, req.ElectionRoundId,
            req.NgoId, ct);
        var dashboard = await context.Dashboards
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == req.DashboardId
                                      && x.ElectionRoundId == req.ElectionRoundId
                                      && x.MonitoringNgoId == monitoringNgoId, ct);
        if (monitoringNgoId is null || dashboard is null)
        {
            return TypedResults.NotFound();
        }

        var model = (await context.ToModelsAsync([dashboard], roleProvider.IsNgoAdmin(), ct)).Single();
        return TypedResults.Ok(model);
    }
}
