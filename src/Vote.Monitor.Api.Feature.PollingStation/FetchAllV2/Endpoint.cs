using Microsoft.Extensions.Caching.Memory;

namespace Vote.Monitor.Api.Feature.PollingStation.FetchAllV2;
public class Endpoint(VoteMonitorContext context, IMemoryCache cache) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations:fetchAllv2");
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

        var cacheKey = $"election-rounds/{request.ElectionRoundId}/polling-stations/{electionRound.PollingStationsVersion}/v2";

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

    private static Dictionary<string, LocationNode> GetLocationNodes(List<PollingStationModel> pollingStations)
    {
        Dictionary<int, Dictionary<string, LocationNode>> levelNodes = new()
        {
            { 1, [] },
            { 2, [] },
            { 3, [] },
            { 4, [] },
            { 5, [] },
        };

        foreach (var pollingStation in pollingStations)
        {
            LocationNode? parentNode = null;
            for (int level = 1; level <= 5; level++)
            {
                string levelValue = GetLevelValue(pollingStation, level);
                if (string.IsNullOrWhiteSpace(levelValue))
                {
                    continue;
                }

                if (!levelNodes[level].TryGetValue(levelValue, out var levelNode))
                {
                    levelNode = new LocationNode
                    {
                        Name = levelValue,
                        Parent = parentNode,
                        PollingStations =
                        {
                            {
                                pollingStation.Number, new()
                                {
                                    Id = pollingStation.Id,
                                    Address = pollingStation.Address
                                }
                            }
                        }
                    };

                    levelNodes[level].Add(levelValue, levelNode);
                }
                else
                {
                    levelNode.PollingStations.TryAdd(pollingStation.Number, new()
                    {
                        Id = pollingStation.Id,
                        Address = pollingStation.Address
                    });
                }
                parentNode = levelNode;
            }
        }

        Dictionary<string, LocationNode> nodes = [];

        for (int level = 5; level >= 1; level--)
        {
            if (levelNodes[level].Any())
            {
                nodes = levelNodes[level];
                break;
            }
        }

        return nodes;
    }

    private static string GetLevelValue(PollingStationModel pollingStation, int level)
    {
        return level switch
        {
            1 => pollingStation.Level1,
            2 => pollingStation.Level2,
            3 => pollingStation.Level3,
            4 => pollingStation.Level4,
            5 => pollingStation.Level5,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, "Unknown level!")
        };
    }
}
