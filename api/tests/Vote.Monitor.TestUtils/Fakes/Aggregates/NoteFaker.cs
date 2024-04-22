using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.NoteAggregate;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class NoteFaker : PrivateFaker<Note>
{
    public NoteFaker(Guid? id = null,
        string? text = null,
        ElectionRound? electionRound = null,
        PollingStation? pollingStation = null,
        MonitoringObserver? monitoringObserver = null)
    {
        UsePrivateConstructor();

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.Text, text ?? string.Empty);
        RuleFor(fake => fake.ElectionRound, electionRound ?? fakeElectionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound?.Id ?? fakeElectionRound.Id);
        RuleFor(fake => fake.PollingStation, pollingStation ?? fakePollingStation);
        RuleFor(fake => fake.PollingStationId, pollingStation?.Id ?? fakePollingStation.Id);
        RuleFor(fake => fake.MonitoringObserver, monitoringObserver ?? fakeMonitoringObserver);
        RuleFor(fake => fake.MonitoringObserverId, monitoringObserver?.Id ?? fakeMonitoringObserver.Id);
    }
}
