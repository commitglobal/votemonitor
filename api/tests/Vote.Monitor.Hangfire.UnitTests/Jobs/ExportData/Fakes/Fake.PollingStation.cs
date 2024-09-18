using System.Text.Json;
using Bogus;
using Vote.Monitor.Hangfire.Jobs.Export.PollingStations.ReadModels;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData.Fakes;

public sealed partial class Fake
{
    private static readonly JsonDocument _emptyTags = JsonDocument.Parse("{}");

    public static PollingStationModel PollingStation(Guid pollingStationId, JsonDocument? tags = null)
    {
        var fakePollingStation = new Faker<PollingStationModel>()
            .RuleFor(x => x.Id, pollingStationId)
            .RuleFor(x => x.Tags, tags ?? _emptyTags)
            .RuleFor(x => x.Level1, f => f.Lorem.Word())
            .RuleFor(x => x.Level2, f => f.Lorem.Word())
            .RuleFor(x => x.Level3, f => f.Lorem.Word())
            .RuleFor(x => x.Level4, f => f.Lorem.Word())
            .RuleFor(x => x.Level5, f => f.Lorem.Word())
            .RuleFor(x => x.Number, f => f.Lorem.Word())
            .RuleFor(x => x.Address, f => f.Lorem.Sentence(10))
            .RuleFor(x => x.DisplayOrder, f => f.Random.Int().ToString());

        return fakePollingStation.Generate();
    }
}
