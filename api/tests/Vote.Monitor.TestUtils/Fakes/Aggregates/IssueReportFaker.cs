using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.IssueReportAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class IssueReportFaker : PrivateFaker<IssueReport>
{
    public IssueReportFaker(Guid? id = null,
        MonitoringObserver? monitoringObserver = null,
        ElectionRound? electionRound = null,
        Form? form = null)
    {
        UsePrivateConstructor();
        electionRound ??= new ElectionRoundAggregateFaker().Generate();
        form ??= new FormAggregateFaker(electionRound).Generate();
        monitoringObserver ??= new MonitoringObserverFaker(electionRound).Generate();

        RuleFor(fake => fake.Id, faker => id ?? faker.Random.Guid());
        RuleFor(fake => fake.ElectionRound, electionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound.Id);
        
        RuleFor(fake => fake.MonitoringObserver, monitoringObserver);
        RuleFor(fake => fake.MonitoringObserverId, monitoringObserver.Id);

        RuleFor(fake => fake.LocationType, IssueReportLocationType.OtherLocation);
        RuleFor(fake => fake.LocationDescription, fake => fake.Address.FullAddress());

        RuleFor(fake => fake.Form, form);
        RuleFor(fake => fake.FormId, form.Id);
    }
}