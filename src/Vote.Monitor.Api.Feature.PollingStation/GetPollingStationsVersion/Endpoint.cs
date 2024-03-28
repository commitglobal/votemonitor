namespace Vote.Monitor.Api.Feature.PollingStation.GetPollingStationsVersion;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations:version");
        DontAutoTag();
        Options(x => x.WithTags("polling-stations", "mobile"));
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await context.ElectionRounds
            .Where(x => x.Id == req.ElectionRoundId)
            .Select(x => new { x.PollingStationsVersion, x.Id })
            .FirstOrDefaultAsync(ct);

        if (electionRound is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new Response
        {
            ElectionRoundId = electionRound.Id,
            CacheKey = electionRound.PollingStationsVersion.ToString()
        });
    }
}
