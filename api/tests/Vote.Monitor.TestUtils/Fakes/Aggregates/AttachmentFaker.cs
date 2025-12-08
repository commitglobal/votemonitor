using Vote.Monitor.Domain.Entities.AttachmentAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class AttachmentFaker : PrivateFaker<Attachment>
{
    public AttachmentFaker(Guid? id = null,
        string? fileName = null,
        string? filePath = null,
        ElectionRound? electionRound = null,
        PollingStation? pollingStation = null,
        MonitoringObserver? monitoringObserver = null,
        bool? isCompleted = null,
        bool? isDeleted = null)
    {
        UsePrivateConstructor();

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.FileName, fileName ?? string.Empty);
        RuleFor(fake => fake.FilePath, filePath ?? string.Empty);
        RuleFor(fake => fake.ElectionRoundId, electionRound?.Id ?? fakeElectionRound.Id);
        RuleFor(fake => fake.PollingStationId, pollingStation?.Id ?? fakePollingStation.Id);
        RuleFor(fake => fake.MonitoringObserver, monitoringObserver ?? fakeMonitoringObserver);
        RuleFor(fake => fake.MonitoringObserverId, monitoringObserver?.Id ?? fakeMonitoringObserver.Id);
        RuleFor(fake => fake.IsCompleted, isCompleted ?? true);
        RuleFor(fake => fake.IsDeleted, isDeleted ?? false);
    }
}
