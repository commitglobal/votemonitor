using Microsoft.Extensions.Caching.Memory;
using Vote.Monitor.Core.Extensions;

namespace Feature.Locations.FetchAll;
public class Endpoint(VoteMonitorContext context, IMemoryCache cache) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/locations:fetchAll");
        DontAutoTag();
        AllowAnonymous();
        Options(x => x.WithTags("locations", "mobile"));
        Description(x => x.Accepts<Request>());
        Summary(s =>
        {
            s.Summary = "Gets all locations for a specific election round";
            s.Description = "Gets all locations and a cache key for the data";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var electionRound = await context.ElectionRounds
            .Where(x => x.Id == request.ElectionRoundId)
            .Select(x => new { x.LocationsVersion, x.Id })
            .FirstOrDefaultAsync(ct);

        if (electionRound is null)
        {
            return TypedResults.NotFound();
        }

        var cacheKey = $"election-rounds/{request.ElectionRoundId}/locations/{electionRound.LocationsVersion}";

        var cachedResponse = await cache.GetOrCreateAsync(cacheKey, async (e) =>
        {
            var locations = await context.Locations
                .Where(x => x.ElectionRoundId == request.ElectionRoundId)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new LocationModel
                {
                    Id = x.Id,
                    Level1 = x.Level1,
                    Level2 = x.Level2,
                    Level3 = x.Level3,
                    Level4 = x.Level4,
                    Level5 = x.Level5,
                    DisplayOrder = x.DisplayOrder
                })
                .ToListAsync(cancellationToken: ct);

            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

            var nodes = GetLocationNodes(locations);

            return new Response
            {
                ElectionRoundId = electionRound.Id,
                Version = electionRound.LocationsVersion.ToString(),
                Nodes = nodes
            };
        });

        return TypedResults.Ok(cachedResponse!);
    }

    private static List<LocationNode> GetLocationNodes(List<LocationModel> locations)
    {
        Dictionary<string, LocationNode> cache = new();
        var result = new List<LocationNode>();
        int id = 0;

        foreach (var location in locations)
        {
            var parentNode = cache.GetOrCreate(BuildKey(location.Level1), () => new LocationNode
            {
                Id = ++id, 
                Name = location.Level1,
                Depth = 1
            });

            if (!string.IsNullOrWhiteSpace(location.Level2))
            {
                var level2Key = BuildKey(location.Level1, location.Level2);
                parentNode = cache.GetOrCreate(level2Key, () => new LocationNode
                {
                    Id = ++id, 
                    Name = location.Level2, 
                    ParentId = parentNode.Id,
                    Depth = 2,
                    DisplayOrder = location.DisplayOrder
                });
            }

            if (!string.IsNullOrWhiteSpace(location.Level3))
            {
                var level3Key = BuildKey(location.Level1, location.Level2, location.Level3);
                parentNode = cache.GetOrCreate(level3Key, () => new LocationNode
                {
                    Id = ++id, 
                    Name = location.Level3, 
                    ParentId = parentNode.Id,
                    Depth = 3
                });
            }

            if (!string.IsNullOrWhiteSpace(location.Level4))
            {
                var level4Key = BuildKey(location.Level1, location.Level2, location.Level3, location.Level4);
                parentNode = cache.GetOrCreate(level4Key, () => new LocationNode
                {
                    Id = ++id, 
                    Name = location.Level4, 
                    ParentId = parentNode.Id,
                    Depth = 4
                });
            }

            if (!string.IsNullOrWhiteSpace(location.Level5))
            {
                var level5Key = BuildKey(location.Level1, location.Level2, location.Level3, location.Level4, location.Level5);
                parentNode = cache.GetOrCreate(level5Key, () => new LocationNode
                {
                    Id = ++id,
                    Name = location.Level5, 
                    ParentId = parentNode.Id,
                    Depth = 5
                });
            }

            result.Add(new LocationNode
            {
                Id = ++id,
                ParentId = parentNode!.Id,
                LocationId = location.Id
            });
        }

        return [.. cache.Values, .. result];
    }

    private static string BuildKey(params string[] keyParts)
    {
        return string.Join("-", keyParts);
    }
}
