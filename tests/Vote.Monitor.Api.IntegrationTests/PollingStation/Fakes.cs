using Vote.Monitor.Api.Feature.ElectionRound;

namespace Vote.Monitor.Api.IntegrationTests.PollingStation;

static class Fakes
{
    public static Feature.PollingStation.Create.Request CreateRequest(this Faker f, ElectionRoundModel electionRound) => new()
    {
        Level1 = f.Address.Country(),
        Level2 = f.Address.County(),
        Level3 = f.Address.City(),
        Level4 = string.Empty,
        Level5 = string.Empty,
        Number = f.Random.Number(100, 10_000).ToString(),
        ElectionRoundId = electionRound.Id,
        Address = f.Address.FullAddress(),
        DisplayOrder = f.Random.Int(0, 100),
        Tags = f.Commerce
            .Categories(4)
            .Select((x, i) => new { Name = $"Tag {i}", Value = x })
            .ToDictionary(x => x.Name, x => x.Value)
    };

    public static Feature.PollingStation.Update.Request UpdateRequest(this Faker f, Guid id, ElectionRoundModel electionRound) => new()
    {
        ElectionRoundId = electionRound.Id,
        Id = id,
        Address = f.Address.FullAddress(),
        DisplayOrder = f.Random.Int(0, 100),
        Tags = f.Commerce
            .Categories(4)
            .Select((x, i) => new { Name = $"Tag {i}", Value = x })
            .ToDictionary(x => x.Name, x => x.Value)
    };
}
