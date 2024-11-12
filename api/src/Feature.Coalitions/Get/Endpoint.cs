using Feature.NgoCoalitions.Models;

namespace Feature.NgoCoalitions.Get;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, Results<Ok<CoalitionModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}");
        DontAutoTag();
        Options(x => x.WithTags("coalitions"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<CoalitionModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var coalition = await context.Coalitions
            .Include(x => x.Memberships)
            .ThenInclude(x => x.MonitoringNgo)
            .ThenInclude(x => x.Ngo)
            .Include(x => x.Leader)
            .ThenInclude(x => x.Ngo)
            .Where(x => x.Id == req.CoalitionId && x.ElectionRoundId == req.ElectionRoundId)
            .Select(x => CoalitionModel.FromEntity(x))
            .FirstOrDefaultAsync(ct);

        if (coalition is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(coalition);
    }
}
