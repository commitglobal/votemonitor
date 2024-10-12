using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class MonitoringObserverFaker : PrivateFaker<MonitoringObserver>
{
    private readonly MonitoringObserverStatus[] _statuses =
        [MonitoringObserverStatus.Active, MonitoringObserverStatus.Suspended];

    public MonitoringObserverFaker(ElectionRound? electionRound = null, MonitoringNgo? monitoringNgo = null,
        ObserverAggregate? observer = null, Guid? id = null, Guid? observerId = null)
    {
        UsePrivateConstructor();

        monitoringNgo ??= new MonitoringNgoAggregateFaker(electionRound: electionRound).Generate();
        observer ??= new ObserverAggregateFaker(observerId).Generate();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.MonitoringNgo, monitoringNgo);
        RuleFor(fake => fake.MonitoringNgoId, monitoringNgo.Id);
        RuleFor(fake => fake.Observer, observer);
        RuleFor(fake => fake.ObserverId, observer.Id);
        RuleFor(fake => fake.Status, fake => fake.PickRandom(_statuses));
        RuleFor(fake => fake.Tags, []);
    }
}