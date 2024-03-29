using Microsoft.Extensions.Caching.Memory;

namespace Vote.Monitor.Api.Feature.PollingStation.FetchAllV3;
public class Endpoint(VoteMonitorContext context, IMemoryCache cache) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations:fetchAllV3");
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

                Dictionary<int, Dictionary<string, LevelModel>> levelValues = new()
                {
                    { 1, [] },
                    { 2, [] },
                    { 3, [] },
                    { 4, [] },
                    { 5, [] },
                };

                foreach (var pollingStation in pollingStations)
                {
                    for (int level = 1; level <= 5; level++)
                    {
                        string levelValue = GetLevelValue(pollingStation, level);
                        if (string.IsNullOrWhiteSpace(levelValue))
                        {
                            continue;
                        }

                        if (levelValues[level].TryGetValue(levelValue, out var node))
                        {
                            continue;
                        }

                        node = new LevelModel(levelValues[level].Count, levelValue, pollingStation.DisplayOrder);
                        levelValues[level].Add(levelValue, node);
                    }
                }

                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

                return new Response
                {
                    ElectionRoundId = electionRound.Id,
                    Version = electionRound.PollingStationsVersion.ToString(),
                    PollingStations = Reindex(pollingStations, levelValues),
                    Level1Values = [.. levelValues[1].Values.OrderBy(x=>x.DisplayOrder)],
                    Level2Values = [.. levelValues[2].Values.OrderBy(x=>x.DisplayOrder)],
                    Level3Values = [.. levelValues[3].Values.OrderBy(x=>x.DisplayOrder)],
                    Level4Values = [.. levelValues[4].Values.OrderBy(x=>x.DisplayOrder)],
                    Level5Values = [.. levelValues[5].Values.OrderBy(x=>x.DisplayOrder)]
                };
            });

        return TypedResults.Ok(cachedResponse!);
    }

    private List<PollingStationModelV3> Reindex(List<PollingStationModel> pollingStations, Dictionary<int, Dictionary<string, LevelModel>> levelValues)
    {
        return pollingStations.Select(x => new PollingStationModelV3()
        {
            Id = x.Id,
            DisplayOrder = x.DisplayOrder,
            Address = x.Address,
            Number = x.Number,
            Level1Id = GetLevelIndexOrDefault(levelValues[1], x.Level1),
            Level2Id = GetLevelIndexOrDefault(levelValues[2], x.Level2),
            Level3Id = GetLevelIndexOrDefault(levelValues[3], x.Level3),
            Level4Id = GetLevelIndexOrDefault(levelValues[4], x.Level4),
            Level5Id = GetLevelIndexOrDefault(levelValues[5], x.Level5)
        }).ToList();
    }

    private int? GetLevelIndexOrDefault(Dictionary<string, LevelModel> levels, string levelValue)
    {
        if (levels.TryGetValue(levelValue, out var level))
        {
            return level.Index;
        }

        return null;
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

public record LevelModel(int Index, string Name, int DisplayOrder);
