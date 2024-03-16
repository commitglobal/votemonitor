namespace Vote.Monitor.Api.IntegrationTests.PollingStation;

static class Fakes
{
    public static Feature.PollingStation.Create.Request CreateRequest(this Faker f) => new()
    {
        ElectionRoundId = f.Random.Guid(),
        Address = f.Address.FullAddress(),
        DisplayOrder = f.Random.Int(0, 100),
        Tags = f.Commerce
            .Categories(4)
            .Select((x, i) => new { Name = $"Tag {i}", Value = x })
            .ToDictionary(x => x.Name, x => x.Value)
    };

    public static Feature.PollingStation.Update.Request UpdateRequest(this Faker f, Guid id) => new()
    {
        ElectionRoundId = Guid.NewGuid(),
        Id = id,
        Address = f.Address.FullAddress(),
        DisplayOrder = f.Random.Int(0, 100),
        Tags = f.Commerce
            .Categories(4)
            .Select((x, i) => new { Name = $"Tag {i}", Value = x })
            .ToDictionary(x => x.Name, x => x.Value)
    };
}
