using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

public class PollingStationInformation : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid PollingStationId { get; private set; }
    public PollingStation PollingStation { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }
    public Guid PollingStationInformationFormId { get; private set; }
    public PollingStationInformationForm PollingStationInformationForm { get; private set; }
    public DateTime? ArrivalTime { get; set; }
    public DateTime? DepartureTime { get; set; }

    public IReadOnlyList<BaseAnswer> Answers { get; private set; } = new List<BaseAnswer>().AsReadOnly();

    private PollingStationInformation(
        ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        PollingStationInformationForm pollingStationInformationForm,
        DateTime? arrivalTime,
        DateTime? departureTime,
        List<BaseAnswer> answers) : base(Guid.NewGuid())
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        PollingStation = pollingStation;
        PollingStationId = pollingStation.Id;
        MonitoringObserver = monitoringObserver;
        MonitoringObserverId = monitoringObserver.Id;
        PollingStationInformationForm = pollingStationInformationForm;
        PollingStationInformationFormId = pollingStationInformationForm.Id;
        ArrivalTime = arrivalTime;
        DepartureTime = departureTime;
        Answers = answers.ToList().AsReadOnly();
    }

    internal static PollingStationInformation Create(
        ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        PollingStationInformationForm pollingStationInformationForm,
        DateTime? arrivalTime,
        DateTime? departureTime,
        List<BaseAnswer> answers) =>
        new(electionRound, pollingStation, monitoringObserver, pollingStationInformationForm, arrivalTime, departureTime, answers);

    internal void UpdateDetails(DateTime? arrivalTime, DateTime? departureTime, IEnumerable<BaseAnswer> answers)
    {
        ArrivalTime = arrivalTime;
        DepartureTime = departureTime;
        Answers = answers.ToList().AsReadOnly();
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private PollingStationInformation()
    {

    }
#pragma warning restore CS8618
}
