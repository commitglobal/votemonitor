using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

public class PollingStationInfo : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid PollingStationId { get; private set; }
    public PollingStation PollingStation { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }
    public Guid PollingStationInfoFormId { get; private set; }
    public PollingStationInfoForm PollingStationInfoForm { get; private set; }
    public string Language { get; private set; }
    public IReadOnlyList<BaseAnswer> Answers { get; private set; } = new List<BaseAnswer>().AsReadOnly();

    private PollingStationInfo(
        ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        PollingStationInfoForm pollingStationInfoForm,
        string language) : base(Guid.NewGuid())
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        PollingStation = pollingStation;
        PollingStationId = pollingStation.Id;
        Language = language;
        MonitoringObserver = monitoringObserver;
        MonitoringObserverId = monitoringObserver.Id;
        PollingStationInfoForm = pollingStationInfoForm;
        PollingStationInfoFormId = pollingStationInfoForm.Id;
    }

    internal static PollingStationInfo Create(
        ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        PollingStationInfoForm pollingStationInfoForm,
        string selectedLanguage) =>
        new(electionRound, pollingStation, monitoringObserver, pollingStationInfoForm, selectedLanguage);

    internal void UpdateDetails(string language, IEnumerable<BaseAnswer> answers)
    {
        Language = language;
        Answers = answers.ToList().AsReadOnly();
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private PollingStationInfo()
    {

    }
#pragma warning restore CS8618
}
