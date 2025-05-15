using Microsoft.Extensions.Caching.Memory;
using Vote.Monitor.Core.Extensions;

namespace Feature.PollingStations.FetchAll;
public class Endpoint(VoteMonitorContext context, IMemoryCache cache) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations:fetchAll");
        DontAutoTag();
        Options(x => x.WithTags("polling-stations", "mobile"));
        Description(x => x.Accepts<Request>());
        Summary(s =>
        {
            s.Summary = "Gets all polling stations for a specific election round";
            s.Description = "Gets all polling stations and a cache key for the data";
        });
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

            var nodes = GetLocationNodes(pollingStations);

            return new Response
            {
                ElectionRoundId = electionRound.Id,
                Version = electionRound.PollingStationsVersion.ToString(),
                Nodes = nodes
            };
        });

        return TypedResults.Ok(cachedResponse!);
    }

    private static List<LocationNode> GetLocationNodes(List<PollingStationModel> pollingStations)
    {
        Dictionary<string, LocationNode> cache = new();
        var result = new List<LocationNode>();
        int id = 0;

        foreach (var ps in pollingStations)
        {
            var parentNode = cache.GetOrCreate(BuildKey(ps.Level1), () => new LocationNode
            {
                Id = ++id, 
                Name = ps.Level1,
                Depth = 1
            });

            if (!string.IsNullOrWhiteSpace(ps.Level2))
            {
                var level2Key = BuildKey(ps.Level1, ps.Level2);
                parentNode = cache.GetOrCreate(level2Key, () => new LocationNode
                {
                    Id = ++id, 
                    Name = ps.Level2, 
                    ParentId = parentNode.Id,
                    Depth = 2
                });
            }

            if (!string.IsNullOrWhiteSpace(ps.Level3))
            {
                var level3Key = BuildKey(ps.Level1, ps.Level2, ps.Level3);
                parentNode = cache.GetOrCreate(level3Key, () => new LocationNode
                {
                    Id = ++id, 
                    Name = ps.Level3, 
                    ParentId = parentNode.Id,
                    Depth = 3
                });
            }

            if (!string.IsNullOrWhiteSpace(ps.Level4))
            {
                var level4Key = BuildKey(ps.Level1, ps.Level2, ps.Level3, ps.Level4);
                parentNode = cache.GetOrCreate(level4Key, () => new LocationNode
                {
                    Id = ++id, 
                    Name = ps.Level4, 
                    ParentId = parentNode.Id,
                    Depth = 4
                });
            }

            if (!string.IsNullOrWhiteSpace(ps.Level5))
            {
                var level5Key = BuildKey(ps.Level1, ps.Level2, ps.Level3, ps.Level4, ps.Level5);
                parentNode = cache.GetOrCreate(level5Key, () => new LocationNode
                {
                    Id = ++id,
                    Name = ps.Level5, 
                    ParentId = parentNode.Id,
                    Depth = 5
                });
            }

            result.Add(new LocationNode
            {
                Id = ++id,
                Name = ps.Address,
                ParentId = parentNode!.Id,
                Number = ps.Number,
                PollingStationId = ps.Id,
                Latitude = ps.Latitude,
                Longitude = ps.Longitude
            });
        }

        return [.. cache.Values, .. result];
    }

    private static string BuildKey(params string[] keyParts)
    {
        return string.Join("-", keyParts);
    }
}
