using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class MonitoringNgoAggregateFaker : PrivateFaker<MonitoringNgo>
{
    private readonly MonitoringNgoStatus[] _statuses = [MonitoringNgoStatus.Active, MonitoringNgoStatus.Suspended];

    public MonitoringNgoAggregateFaker(Guid? id = null,
        ElectionRound? electionRound = null,
        Ngo? ngo = null,
        List<MonitoringObserver>? observers = null,
        MonitoringNgoStatus? status = null)
    {
        UsePrivateConstructor();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        ngo ??= new NgoAggregateFaker().Generate();
        electionRound ??= new ElectionRoundAggregateFaker().Generate();

        RuleFor(fake => fake.Ngo, ngo);
        RuleFor(fake => fake.NgoId, ngo.Id);
        RuleFor(fake => fake.ElectionRound, electionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound.Id);
        RuleFor(fake => fake.MonitoringObservers, observers ?? []);
        RuleFor(fake => fake.Status, fake => status ?? fake.PickRandom(_statuses));
    }
}
