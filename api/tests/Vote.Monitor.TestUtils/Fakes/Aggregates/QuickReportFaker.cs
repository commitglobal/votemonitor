using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class QuickReportFaker : PrivateFaker<QuickReport>
{
    public QuickReportFaker(Guid? id = null,
        ElectionRound? electionRound = null,
        MonitoringObserver? monitoringObserver = null)
    {
        UsePrivateConstructor();

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.ElectionRound, electionRound ?? fakeElectionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound?.Id ?? fakeElectionRound.Id);

        RuleFor(fake => fake.Title, f => f.Lorem.Paragraph(1));
        RuleFor(fake => fake.Description, f => f.Lorem.Paragraph());
        RuleFor(fake => fake.QuickReportLocationType, QuickReportLocationType.NotRelatedToAPollingStation);

        RuleFor(fake => fake.MonitoringObserver, monitoringObserver ?? fakeMonitoringObserver);
        RuleFor(fake => fake.MonitoringObserverId, monitoringObserver?.Id ?? fakeMonitoringObserver.Id);
    }
}
