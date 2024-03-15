using Vote.Monitor.Domain.Entities.PollingStationNoteAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public class PollingStationNoteFaker : PrivateFaker<PollingStationNote>
{
    public PollingStationNoteFaker(Guid? id = null,
        string? text = null)
    {
        UsePrivateConstructor();

        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var pollingStation = new PollingStationFaker().Generate();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.ElectionRound, electionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound.Id);
        RuleFor(fake => fake.PollingStation, pollingStation);
        RuleFor(fake => fake.PollingStationId, pollingStation.Id);
        RuleFor(fake => fake.Text, text ?? string.Empty);
    }
}
