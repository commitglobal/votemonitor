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

        var currentUtcTimeProvider = new CurrentUtcTimeProvider();

        CustomInstantiator(f => new PollingStation(id ?? f.Random.Guid(),
            electionRound: electionRound ?? new ElectionRoundAggregateFaker().Generate(),
            address: f.Address.FullAddress(),
            displayOrder: f.IndexFaker,
            tags: JsonSerializer.SerializeToDocument("")));
    }
}
