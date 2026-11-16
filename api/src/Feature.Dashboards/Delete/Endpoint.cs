namespace Feature.Dashboards.Delete;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/dashboards/{dashboardId}");
        DontAutoTag();
        Options(x => x.WithTags("dashboards"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Deletes one of the caller's NGO's dashboards";
            s.Description = "NGO admins, also in archived rounds. Another NGO's dashboard is 404.";
        });
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
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

        context.Dashboards.Remove(dashboard);
        await context.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
