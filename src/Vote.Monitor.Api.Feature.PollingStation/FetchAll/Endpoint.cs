using Microsoft.Extensions.Caching.Memory;

namespace Vote.Monitor.Api.Feature.PollingStation.FetchAll;
public class Endpoint(VoteMonitorContext context, IMemoryCache cache) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations:fetchAll");
        DontAutoTag();
        Options(x => x.WithTags("polling-stations", "mobile"));
        Description(x => x.Accepts<Request>());
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var electionRound = await context.ElectionRounds
            .Where(x => x.Id == request.ElectionRoundId)
            .Select(x => new { x.PollingStationsVersion, x.Id })
            .FirstOrDefaultAsync(ct);

        if (electionRound is null)
        {
            return TypedResults.NotFound();
        }

        var cacheKey = $"election-rounds/{request.ElectionRoundId}/polling-stations/{electionRound.PollingStationsVersion}";

        var cachedResponse = await cache.GetOrCreateAsync(cacheKey, async (e) =>
            {
                var pollingStations = await context.PollingStations
                    .Where(x => x.ElectionRoundId == request.ElectionRoundId)
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new PollingStationModel
                    {
                        Id = x.Id,
                        Level1 = x.Level1,
                        Level2 = x.Level2,
                        Level3 = x.Level3,
                        Level4 = x.Level4,
                        Level5 = x.Level5,
                        Number = x.Number,
                        Address = x.Address,
                        DisplayOrder = x.DisplayOrder
                    })
                    .ToListAsync(cancellationToken: ct);

                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

                return new Response
                { 
                    ElectionRoundId = electionRound.Id,
                    Version = electionRound.PollingStationsVersion.ToString(),
                    PollingStations = pollingStations
                };
            });

        return TypedResults.Ok(cachedResponse!);
    }
}
