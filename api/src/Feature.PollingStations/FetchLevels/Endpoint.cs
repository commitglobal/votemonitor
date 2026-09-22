using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.Core.Extensions;
using ZiggyCreatures.Caching.Fusion;

namespace Feature.PollingStations.FetchLevels;
public class Endpoint(VoteMonitorContext context, IFusionCache cache, IServiceScopeFactory scopeFactory) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations:fetchLevels");
        DontAutoTag();
        Options(x => x.WithTags("polling-stations"));
        Description(x => x.Accepts<Request>());
        Summary(s =>
        {
            s.Summary = "Gets all levels for all polling stations";
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

        var cacheKey = $"election-rounds/{request.ElectionRoundId}/polling-station-nodes/{electionRound.PollingStationsVersion}";

        var cachedResponse = await cache.GetOrSetAsync(
            cacheKey,
            async factoryCt =>
            {
                // Fresh scope: FusionCache eager refresh may outlive the request DbContext.
                await using var scope = scopeFactory.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<VoteMonitorContext>();

                var pollingStations = await db.PollingStations
                    .Where(x => x.ElectionRoundId == request.ElectionRoundId)
                    .Select(x => new 
                    {
                        x.Level1,
                        x.Level2,
                        x.Level3,
                        x.Level4,
                        x.Level5
                    })
                    .Distinct()
                    .ToListAsync(cancellationToken: factoryCt);

                Dictionary<string, LevelNode> nodes = new();
                int id = 0;

                foreach (var ps in pollingStations)
                {
                    var parentNode = nodes.GetOrCreate(BuildKey(ps.Level1), () => new LevelNode
                    {
                        Id = ++id,
                        Name = ps.Level1,
                        Depth = 1
                    });

                    if (!string.IsNullOrWhiteSpace(ps.Level2))
                    {
                        var level2Key = BuildKey(ps.Level1, ps.Level2);
                        parentNode = nodes.GetOrCreate(level2Key, () => new LevelNode
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
                        parentNode = nodes.GetOrCreate(level3Key, () => new LevelNode
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
                        parentNode = nodes.GetOrCreate(level4Key, () => new LevelNode
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
                        parentNode = nodes.GetOrCreate(level5Key, () => new LevelNode
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
                    Version = electionRound.PollingStationsVersion.ToString(),
                    Nodes = [.. nodes.Values]
                };
            },
            options => options.SetDuration(TimeSpan.FromMinutes(30)),
            token: ct);

        return TypedResults.Ok(cachedResponse);
    }

    private static string BuildKey(params string[] keyParts)
    {
        return string.Join("-", keyParts);
    }
}
