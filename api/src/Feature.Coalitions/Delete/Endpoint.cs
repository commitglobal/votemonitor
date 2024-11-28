namespace Feature.NgoCoalitions.Delete;

public class Endpoint(VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound, Conflict>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/coalitions/{coalitionId}");
        DontAutoTag();
        Options(x => x.WithTags("coalitions"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound, Conflict>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var coalition = await context.Coalitions
            .Include(x => x.Memberships)
            .Where(x => x.Id == req.CoalitionId && x.ElectionRoundId == req.ElectionRoundId)
            .FirstOrDefaultAsync(ct);

        if (coalition == null)
        {
            return TypedResults.NotFound();
        }

        context.Coalitions.Remove(coalition);
        await context.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
