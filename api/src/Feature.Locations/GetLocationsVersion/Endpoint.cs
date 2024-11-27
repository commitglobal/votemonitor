namespace Feature.Locations.GetLocationsVersion;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/locations:version");
        DontAutoTag();
        Options(x => x.WithTags("locations", "mobile"));
        Summary(s =>
        {
            s.Summary = "Gets current version of locations for an election round";
            s.Description = "Cache key changes every time any location changes";
        });
        AllowAnonymous();
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await context.ElectionRounds
            .Where(x => x.Id == req.ElectionRoundId)
            .Select(x => new { x.LocationsVersion, x.Id })
            .FirstOrDefaultAsync(ct);

        if (electionRound is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new Response
        {
            ElectionRoundId = electionRound.Id,
            CacheKey = electionRound.LocationsVersion.ToString()
        });
    }
}
