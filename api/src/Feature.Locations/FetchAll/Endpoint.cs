using Microsoft.Extensions.Caching.Memory;
using NPOI.SS.Formula;
using Vote.Monitor.Core.Extensions;

namespace Feature.Locations.FetchAll;

public class Endpoint(VoteMonitorContext context, IMemoryCache cache)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
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
        // Root node to store the top-level location nodes
        LocationNode root = new LocationNode(0, "Root", 0, -1, 0);
        Dictionary<string, LocationNode> cache = new(); // Cache to store nodes and avoid duplicates
        int id = 0;

        foreach (var location in locations)
        {
            // Create the hierarchy for Level1 -> Level2 -> Level3 -> Level4 -> Level5
            var level1Key = location.Level1;
            var level2Key = $"{location.Level1}-{location.Level2}";
            var level3Key = $"{level2Key}-{location.Level3}";
            var level4Key = $"{level3Key}-{location.Level4}";
            var level5Key = $"{level4Key}-{location.Level5}";
            // Level 1
            var level1Node = cache.GetOrCreate(level1Key, () =>
            {
                var node = new LocationNode(++id, location.Level1, 1, root.Id, location.DisplayOrder);
                root.AddChild(node);
                return node;
            });

            // Level 2 (optional)
            LocationNode level2Node = null;
            if (!string.IsNullOrWhiteSpace(location.Level2))
            {
                level2Node = cache.GetOrCreate(level2Key, () =>
                {
                    var node = new LocationNode(++id, location.Level2, 2, level1Node.Id, location.DisplayOrder);
                    level1Node.AddChild(node);
                    return node;
                });
            }

            // Level 3 (optional)
            LocationNode level3Node = null;
            if (!string.IsNullOrWhiteSpace(location.Level3))
            {
                level3Node = cache.GetOrCreate(level3Key, () =>
                {
                    var node = new LocationNode(++id, location.Level3, 3, (level2Node ?? level1Node).Id,
                        location.DisplayOrder);
                    (level2Node ?? level1Node).AddChild(node);
                    return node;
                });
            }

            // Level 4 (optional)
            LocationNode level4Node = null;
            if (!string.IsNullOrWhiteSpace(location.Level4))
            {
                level4Node = cache.GetOrCreate(level4Key, () =>
                {
                    var node = new LocationNode(++id, location.Level4, 4, (level3Node ?? level2Node ?? level1Node).Id,
                        location.DisplayOrder);
                    (level3Node ?? level2Node ?? level1Node).AddChild(node);
                    return node;
                });
            }

            // Level 5 (optional)
            if (!string.IsNullOrWhiteSpace(location.Level5))
            {
                cache.GetOrCreate(level5Key, () =>
                {
                    var node = new LocationNode(++id, location.Level5, 5,
                        (level4Node ?? level3Node ?? level2Node ?? level1Node).Id, location.DisplayOrder);
                    (level4Node ?? level3Node ?? level2Node ?? level1Node).AddChild(node);
                    return node;
                });
            }
        }

        return root.Traverse().Where(x => x.Id != 0).ToList();
    }
}