using Microsoft.Extensions.Caching.Memory;
using Vote.Monitor.Core.Extensions;

namespace Feature.Locations.FetchLevels;
public class Endpoint(VoteMonitorContext context, IMemoryCache memoryCache) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/locations:fetchLevels");
        DontAutoTag();
        Options(x => x.WithTags("locations"));
        Description(x => x.Accepts<Request>());
        Summary(s =>
        {
            s.Summary = "Gets all levels for all locations";
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

        var cacheKey = $"election-rounds/{request.ElectionRoundId}/location-nodes/{electionRound.LocationsVersion}";

        var cachedResponse = await memoryCache.GetOrCreateAsync(cacheKey, async (e) =>
        {
            var locations = await context.Locations
                .Where(x => x.ElectionRoundId == request.ElectionRoundId)
                .Select(x => new 
                {
                    x.Level1,
                    x.Level2,
                    x.Level3,
                    x.Level4,
                    x.Level5,
                })
                .Distinct()
                .ToListAsync(cancellationToken: ct);

            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

            Dictionary<string, LevelNode> cache = new();
            int id = 0;

            foreach (var ps in locations)
            {
                var parentNode = cache.GetOrCreate(BuildKey(ps.Level1), () => new LevelNode
                {
                    Id = ++id,
                    Name = ps.Level1,
                    Depth = 1
                });

                if (!string.IsNullOrWhiteSpace(ps.Level2))
                {
                    var level2Key = BuildKey(ps.Level1, ps.Level2);
                    parentNode = cache.GetOrCreate(level2Key, () => new LevelNode
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
                    parentNode = cache.GetOrCreate(level3Key, () => new LevelNode
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
                    parentNode = cache.GetOrCreate(level4Key, () => new LevelNode
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
                    parentNode = cache.GetOrCreate(level5Key, () => new LevelNode
                    {
                        Id = ++id,
                        Name = ps.Level5,
                        ParentId = parentNode.Id,
                        Depth = 5
                    });
                }
            }

            return new Response
            {
                ElectionRoundId = electionRound.Id,
                Version = electionRound.LocationsVersion.ToString(),
                Nodes = [.. cache.Values]
            };
        });

        return TypedResults.Ok(cachedResponse!);
    }

    private static string BuildKey(params string[] keyParts)
    {
        return string.Join("-", keyParts);
    }
}
