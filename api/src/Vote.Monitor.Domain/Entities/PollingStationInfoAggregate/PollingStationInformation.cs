using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
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
    public DateTime? ArrivalTime { get; private set; }
    public DateTime? DepartureTime { get; private set; }
    public double? MinutesMonitoring { get; private set; }
    public int NumberOfQuestionsAnswered { get; private set; }
    public int NumberOfFlaggedAnswers { get; private set; }
    public SubmissionFollowUpStatus FollowUpStatus { get; private set; }
    public IReadOnlyList<BaseAnswer> Answers { get; private set; } = new List<BaseAnswer>().AsReadOnly();
    public IReadOnlyList<ObservationBreak> Breaks { get; private set; } = new List<ObservationBreak>().AsReadOnly();

    private PollingStationInformation(
        ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        PollingStationInformationForm pollingStationInformationForm,
        DateTime? arrivalTime,
        DateTime? departureTime,
        List<BaseAnswer> answers,
        int numberOfQuestionsAnswered,
        int numberOfFlaggedAnswers,
        List<ObservationBreak> breaks,
        int version = 1) : base(Guid.NewGuid())
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        PollingStation = pollingStation;
        PollingStationId = pollingStation.Id;
        MonitoringObserver = monitoringObserver;
        MonitoringObserverId = monitoringObserver.Id;
        PollingStationInformationForm = pollingStationInformationForm;
        PollingStationInformationFormId = pollingStationInformationForm.Id;
        Answers = answers.ToList().AsReadOnly();
        NumberOfQuestionsAnswered = numberOfQuestionsAnswered;
        NumberOfFlaggedAnswers = numberOfFlaggedAnswers;
        FollowUpStatus = SubmissionFollowUpStatus.NotApplicable;

        if (version == 1)
        {
            UpdateTimesOfStay(arrivalTime, departureTime, breaks);
        }
        else
        {
            UpdateTimesOfStayV2(arrivalTime, departureTime, breaks);
        }
    }

    [Obsolete("Will be removed after 27.10.2024")]
    internal static PollingStationInformation Create(
        ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        PollingStationInformationForm pollingStationInformationForm,
        DateTime? arrivalTime,
        DateTime? departureTime,
        List<BaseAnswer> answers,
        int numberOfQuestionsAnswered,
        int numberOfFlaggedAnswers,
        List<ObservationBreak> breaks) =>
        new(electionRound, pollingStation, monitoringObserver, pollingStationInformationForm, arrivalTime,
            departureTime, answers, numberOfQuestionsAnswered, numberOfFlaggedAnswers, breaks);

    internal static PollingStationInformation CreateV2(
        ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        PollingStationInformationForm pollingStationInformationForm,
        DateTime? arrivalTime,
        DateTime? departureTime,
        List<BaseAnswer> answers,
        int numberOfQuestionsAnswered,
        int numberOfFlaggedAnswers,
        List<ObservationBreak> breaks) =>
        new(electionRound, pollingStation, monitoringObserver, pollingStationInformationForm, arrivalTime,
            departureTime, answers, numberOfQuestionsAnswered, numberOfFlaggedAnswers, breaks, 2);

    [Obsolete("Will be removed after 27.10.2024")]
    internal void UpdateAnswers(IEnumerable<BaseAnswer> answers,
        int numberOfQuestionsAnswered,
        int numberOfFlaggedAnswers,
        DateTime? arrivalTime,
        DateTime? departureTime,
        List<ObservationBreak> breaks)
    {
        Answers = answers.ToList().AsReadOnly();
        NumberOfQuestionsAnswered = numberOfQuestionsAnswered;
        NumberOfFlaggedAnswers = numberOfFlaggedAnswers;

        UpdateTimesOfStay(arrivalTime, departureTime, breaks);
    }

    internal void UpdateAnswersV2(IEnumerable<BaseAnswer> answers,
        int numberOfQuestionsAnswered,
        int numberOfFlaggedAnswers,
        DateTime? arrivalTime,
        DateTime? departureTime,
        List<ObservationBreak> breaks)
    {
        Answers = answers.ToList().AsReadOnly();
        NumberOfQuestionsAnswered = numberOfQuestionsAnswered;
        NumberOfFlaggedAnswers = numberOfFlaggedAnswers;

        UpdateTimesOfStayV2(arrivalTime, departureTime, breaks);
    }

    [Obsolete("Will be removed after 27.10.2024")]
    public void UpdateTimesOfStay(DateTime? arrivalTime, DateTime? departureTime, IEnumerable<ObservationBreak> breaks)
    {
        if (arrivalTime.HasValue)
        {
            ArrivalTime = arrivalTime;
        }

        if (departureTime.HasValue)
        {
            DepartureTime = departureTime;
        }

        if (departureTime.HasValue && arrivalTime.HasValue && departureTime >= arrivalTime)
        {
            MinutesMonitoring = (departureTime.Value - arrivalTime.Value).TotalMinutes;
        }

        Breaks = breaks.ToList().AsReadOnly();
    }

    public void UpdateTimesOfStayV2(DateTime? arrivalTime,
        DateTime? departureTime,
        IEnumerable<ObservationBreak> breaks)
    {
        ArrivalTime = arrivalTime;
        DepartureTime = departureTime;

        if (departureTime.HasValue && arrivalTime.HasValue && departureTime >= arrivalTime)
        {
            MinutesMonitoring = (departureTime.Value - arrivalTime.Value).TotalMinutes;
        }

        Breaks = breaks.ToList().AsReadOnly();
    }

    public void ClearAnswers()
    {
        Answers = [];
        NumberOfQuestionsAnswered = 0;
        NumberOfFlaggedAnswers = 0;
    }

    public void UpdateFollowUpStatus(SubmissionFollowUpStatus followUpStatus)
    {
        FollowUpStatus = followUpStatus;
    }


#pragma warning disable CS8618 // Required by Entity Framework

    private PollingStationInformation()
    {
    }
#pragma warning restore CS8618
}