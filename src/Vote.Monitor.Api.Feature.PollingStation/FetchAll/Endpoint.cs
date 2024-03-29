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
        Summary(s => {
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
        Dictionary<string, LocationNode> lvl1Cache = new();
        Dictionary<string, LocationNode> lvl2Cache = new();
        Dictionary<string, LocationNode> lvl3Cache = new();
        Dictionary<string, LocationNode> lvl4Cache = new();
        Dictionary<string, LocationNode> lvl5Cache = new();

        var result = new List<LocationNode>();
        int id = 0;

        foreach (var pollingStation in pollingStations)
        {
            LocationNode currentParent = null;
            if (!lvl1Cache.ContainsKey(pollingStation.Level1))
            {
                currentParent = new LocationNode
                {
                    Id = ++id,
                    Name = pollingStation.Level1
                };
                result.Add(currentParent);
                lvl1Cache.TryAdd(pollingStation.Level1, currentParent);
            }
            else
            {
                currentParent = lvl1Cache[pollingStation.Level1];
            }

            if (!string.IsNullOrEmpty(pollingStation.Level2))
            {
                if (!lvl2Cache.ContainsKey(pollingStation.Level2))
                {
                    currentParent = new LocationNode
                    {
                        Id = ++id,
                        Name = pollingStation.Level2,
                        ParentId = lvl1Cache[pollingStation.Level1].Id
                    };
                    result.Add(currentParent);

                    lvl2Cache.TryAdd(pollingStation.Level2, currentParent);
                }
                else
                {
                    currentParent = lvl2Cache[pollingStation.Level2];
                }
            }

            if (!string.IsNullOrEmpty(pollingStation.Level3))
            {
                if (!lvl3Cache.ContainsKey(pollingStation.Level3))
                {
                    currentParent = new LocationNode
                    {
                        Id = ++id,
                        Name = pollingStation.Level3,
                        ParentId = lvl2Cache[pollingStation.Level2].Id
                    };

                    result.Add(currentParent);
                    lvl3Cache.TryAdd(pollingStation.Level3, currentParent);
                }
                else
                {
                    currentParent = lvl3Cache[pollingStation.Level3];
                }
            }

            if (!string.IsNullOrEmpty(pollingStation.Level4))
            {
                if (!lvl4Cache.ContainsKey(pollingStation.Level4))
                {
                    currentParent = new LocationNode
                    {
                        Id = ++id,
                        Name = pollingStation.Level4,
                        ParentId = lvl3Cache[pollingStation.Level3].Id
                    };

                    result.Add(currentParent);
                    lvl4Cache.TryAdd(pollingStation.Level4, currentParent);
                }
                else
                {
                    currentParent = lvl4Cache[pollingStation.Level4];
                }
            }

            if (!string.IsNullOrEmpty(pollingStation.Level5))
            {
                if (!lvl5Cache.ContainsKey(pollingStation.Level5))
                {
                    currentParent = new LocationNode
                    {
                        Id = ++id,
                        Name = pollingStation.Level5,
                        ParentId = lvl4Cache[pollingStation.Level4].Id
                    };

                    result.Add(currentParent);
                    lvl5Cache.Add(pollingStation.Level5, currentParent);
                }
                else
                {
                    currentParent = lvl5Cache[pollingStation.Level5];
                }
            }

            result.Add(new LocationNode
            {
                Id = ++id,
                Name = pollingStation.Address,
                ParentId = currentParent!.Id,
                Number = pollingStation.Number,
                PollingStationId = pollingStation.Id
            });
        }

        return result;
    }
}
