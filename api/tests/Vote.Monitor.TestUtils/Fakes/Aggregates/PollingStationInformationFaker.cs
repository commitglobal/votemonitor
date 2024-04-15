using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class PollingStationInformationFaker : PrivateFaker<PollingStationInformation>
{
    public PollingStationInformationFaker(Guid? id = null,
        ElectionRound? electionRound = null,
        PollingStation? pollingStation = null,
        MonitoringObserver? monitoringObserver = null,
        PollingStationInformationForm? pollingStationInformationForm = null)
    {
        UsePrivateConstructor();
        electionRound ??= new ElectionRoundAggregateFaker().Generate();
        pollingStation ??= new PollingStationFaker().Generate();
        monitoringObserver ??= new MonitoringObserverFaker().Generate();
        pollingStationInformationForm ??= new PollingStationInformationFormFaker(electionRound).Generate();

        RuleFor(fake => fake.Id, faker => id ?? faker.Random.Guid());
        RuleFor(fake => fake.ElectionRound, electionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound.Id);

        RuleFor(fake => fake.PollingStation, pollingStation);
        RuleFor(fake => fake.PollingStationId, pollingStation.Id);

        RuleFor(fake => fake.MonitoringObserver, monitoringObserver);
        RuleFor(fake => fake.MonitoringObserverId, monitoringObserver.Id);

        RuleFor(fake => fake.PollingStationInformationForm, pollingStationInformationForm);
        RuleFor(fake => fake.PollingStationInformationFormId, pollingStationInformationForm.Id);
    }
}
