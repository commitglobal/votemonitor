using System.Text.Json;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class PollingStationFaker : PrivateFaker<PollingStation>
{
    public PollingStationFaker(Guid? id = null,
        ElectionRound? electionRound = null,
        string? address = null,
        int? displayOrder = null,
        JsonDocument? tags = null)
    {
        UsePrivateConstructor();

        CustomInstantiator(f => new PollingStation(id ?? f.Random.Guid(),
            electionRound: electionRound ?? new ElectionRoundAggregateFaker().Generate(),
            level1: f.Address.County(),
            level2: f.Address.City(),
            level3: null,
            level4: null,
            level5: null,
            number: f.Random.Number(1, 1000).ToString(),
            address: f.Address.FullAddress(),
            displayOrder: f.IndexFaker,
            tags: JsonSerializer.SerializeToDocument(""),
            latitude: f.Address.Latitude(),
            longitude: f.Address.Longitude()));
    }
}
