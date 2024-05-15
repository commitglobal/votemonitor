using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class FormSubmissionFaker : PrivateFaker<FormSubmission>
{
    public FormSubmissionFaker(Guid? id = null,
        ElectionRound? electionRound = null,
        PollingStation? pollingStation = null,
        MonitoringObserver? monitoringObserver = null,
        Form? form = null)
    {
        UsePrivateConstructor();
        electionRound ??= new ElectionRoundAggregateFaker().Generate();
        pollingStation ??= new PollingStationFaker().Generate();
        monitoringObserver ??= new MonitoringObserverFaker().Generate();
        form ??= new FormAggregateFaker(electionRound).Generate();

        RuleFor(fake => fake.Id, faker => id ?? faker.Random.Guid());
        RuleFor(fake => fake.ElectionRound, electionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound.Id);

        RuleFor(fake => fake.PollingStation, pollingStation);
        RuleFor(fake => fake.PollingStationId, pollingStation.Id);

        RuleFor(fake => fake.MonitoringObserver, monitoringObserver);
        RuleFor(fake => fake.MonitoringObserverId, monitoringObserver.Id);

        RuleFor(fake => fake.Form, form);
        RuleFor(fake => fake.FormId, form.Id);
    }
}
