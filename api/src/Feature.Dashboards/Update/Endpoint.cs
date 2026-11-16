using Feature.Dashboards.Models;

namespace Feature.Dashboards.Update;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context) : Endpoint<Request, Results<Ok<DashboardModel>, NotFound>>
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
                            "Another NGO's dashboard is 404.";
        });
    }

    public override async Task<Results<Ok<DashboardModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
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

        dashboard.Update(req.Name.Trim(),
            DashboardCharts.Description(req.Description),
            DashboardCharts.ToDocument(req.Charts));
        await context.SaveChangesAsync(ct);

        var model = (await context.ToModelsAsync([dashboard], true, ct)).Single();
        return TypedResults.Ok(model);
    }
}
