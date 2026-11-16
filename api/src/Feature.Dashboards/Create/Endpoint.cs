using Feature.Dashboards.Models;

namespace Feature.Dashboards.Create;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context) : Endpoint<Request, Results<Created<DashboardModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/dashboards");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("dashboards"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Creates a dashboard for the caller's NGO";
            s.Description =
                "NGO admins, also in archived rounds. The charts are stored as given and returned unchanged. An NGO " +
                $"can have at most {Dashboard.MaxDashboardsPerRound} dashboards per round.";
        });
    }

    public override async Task<Results<Created<DashboardModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var monitoringNgoId = await context.ResolveMonitoringNgoIdAsync(authorizationService, User, req.ElectionRoundId,
            req.NgoId, ct);
        var creatorName = await context.Users
            .Where(x => x.Id == req.UserId)
            .Select(x => x.DisplayName)
            .FirstOrDefaultAsync(ct);
        if (monitoringNgoId is null || creatorName is null)
        {
            return TypedResults.NotFound();
        }

        var count = await context.Dashboards.CountAsync(x => x.ElectionRoundId == req.ElectionRoundId
                                                             && x.MonitoringNgoId == monitoringNgoId, ct);
        if (count >= Dashboard.MaxDashboardsPerRound)
        {
            AddError(x => x.Name,
                $"Your organization can have at most {Dashboard.MaxDashboardsPerRound} dashboards in this round");
        }

        ThrowIfAnyErrors();

        var dashboard = Dashboard.Create(req.ElectionRoundId,
            monitoringNgoId.Value,
            req.Name.Trim(),
            DashboardCharts.Description(req.Description),
            DashboardCharts.ToDocument(req.Charts),
            req.UserId,
            creatorName);
        context.Dashboards.Add(dashboard);
        await context.SaveChangesAsync(ct);

        var model = (await context.ToModelsAsync([dashboard], true, ct)).Single();
        return TypedResults.Created($"/api/election-rounds/{req.ElectionRoundId}/dashboards/{dashboard.Id}", model);
    }
}
