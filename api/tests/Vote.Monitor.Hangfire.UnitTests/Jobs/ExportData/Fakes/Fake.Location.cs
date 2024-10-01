using System.Text.Json;
using Bogus;
using Vote.Monitor.Hangfire.Jobs.Export.Locations.ReadModels;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData.Fakes;

public sealed partial class Fake
{
    public static LocationModel Location(Guid pollingStationId, JsonDocument? tags = null)
    {
        var fakePollingStation = new Faker<LocationModel>()
            .RuleFor(x => x.Id, pollingStationId)
            .RuleFor(x => x.Level1, f => f.Lorem.Word())
            .RuleFor(x => x.Level2, f => f.Lorem.Word())
            .RuleFor(x => x.Level3, f => f.Lorem.Word())
            .RuleFor(x => x.Level4, f => f.Lorem.Word())
            .RuleFor(x => x.Level5, f => f.Lorem.Word())
            .RuleFor(x => x.DisplayOrder, f => f.Random.Int().ToString())
            .RuleFor(x => x.Tags, tags ?? _emptyTags);

        return fakePollingStation.Generate();
    }
}