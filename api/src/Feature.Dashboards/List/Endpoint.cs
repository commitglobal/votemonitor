using Feature.Dashboards.Models;

namespace Feature.Dashboards.List;

public class Endpoint(
    IAuthorizationService authorizationService,
    ICurrentUserRoleProvider roleProvider,
    VoteMonitorContext context) : Endpoint<Request, Results<Ok<DashboardsModel>, NotFound>>
{
    private static readonly StringComparer IgnoreCase = StringComparer.OrdinalIgnoreCase;

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/dashboards");
        DontAutoTag();
        Options(x => x.WithTags("dashboards"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Lists the caller's NGO's dashboards in a round";
            s.Description = "Every dashboard of the NGO, A to Z ignoring case, with its charts. NGO admins and staff, " +
                            "also in archived rounds.";
        });
    }

    public override async Task<Results<Ok<DashboardsModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var monitoringNgoId = await context.ResolveMonitoringNgoIdAsync(authorizationService, User, req.ElectionRoundId,
            req.NgoId, ct);
        if (monitoringNgoId is null)
        {
            return TypedResults.NotFound();
        }

        var dashboards = await context.Dashboards
            .AsNoTracking()
            .Where(x => x.ElectionRoundId == req.ElectionRoundId && x.MonitoringNgoId == monitoringNgoId)
            .ToListAsync(ct);

        var ordered = dashboards
            .OrderBy(x => x.Name, IgnoreCase)
            .ThenBy(x => x.Name, StringComparer.Ordinal)
            .ThenBy(x => x.CreatedOn)
            .ToList();

        var models = await context.ToModelsAsync(ordered, roleProvider.IsNgoAdmin(), ct);
        return TypedResults.Ok(new DashboardsModel(models));
    }
}
