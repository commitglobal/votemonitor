using Vote.Monitor.Core.Models;
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
    public bool IsCompleted { get; private set; }

    private PollingStationInformation(
        ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        PollingStationInformationForm pollingStationInformationForm,
        ValueOrUndefined<DateTime?> arrivalTime,
        ValueOrUndefined<DateTime?> departureTime,
        List<BaseAnswer> answers,
        int numberOfQuestionsAnswered,
        int numberOfFlaggedAnswers,
        List<ObservationBreak>? breaks,
        ValueOrUndefined<bool> isCompleted) : base(Guid.NewGuid())
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        PollingStation = pollingStation;
        PollingStationId = pollingStation.Id;
        MonitoringObserver = monitoringObserver;
        MonitoringObserverId = monitoringObserver.Id;
        PollingStationInformationForm = pollingStationInformationForm;
        PollingStationInformationFormId = pollingStationInformationForm.Id;
        NumberOfQuestionsAnswered = numberOfQuestionsAnswered;
        NumberOfFlaggedAnswers = numberOfFlaggedAnswers;
        FollowUpStatus = SubmissionFollowUpStatus.NotApplicable;

        Answers = answers.ToList().AsReadOnly();

        if (!isCompleted.IsUndefined)
        {
            IsCompleted = isCompleted.Value;
        }

        UpdateTimesOfStay(arrivalTime, departureTime, breaks);
    }

    internal static PollingStationInformation Create(
        ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        PollingStationInformationForm pollingStationInformationForm,
        ValueOrUndefined<DateTime?> arrivalTime,
        ValueOrUndefined<DateTime?> departureTime,
        List<BaseAnswer> answers,
        int numberOfQuestionsAnswered,
        int numberOfFlaggedAnswers,
        List<ObservationBreak>? breaks,
        ValueOrUndefined<bool> isCompleted) =>
        new(electionRound, pollingStation, monitoringObserver, pollingStationInformationForm, arrivalTime,
            departureTime, answers, numberOfQuestionsAnswered, numberOfFlaggedAnswers, breaks, isCompleted);

    internal void Update(IEnumerable<BaseAnswer>? answers,
        int? numberOfQuestionsAnswered,
        int? numberOfFlaggedAnswers,
        ValueOrUndefined<DateTime?> arrivalTime,
        ValueOrUndefined<DateTime?> departureTime,
        List<ObservationBreak>? breaks,
        ValueOrUndefined<bool> isCompleted)
    {
        if (answers is not null)
        {
            Answers = answers.ToList().AsReadOnly();
        }

        if (numberOfFlaggedAnswers is not null)
        {
            NumberOfFlaggedAnswers = numberOfFlaggedAnswers.Value;
        }

        if (numberOfQuestionsAnswered is not null)
        {
            NumberOfQuestionsAnswered = numberOfQuestionsAnswered.Value;
        }

        if (!isCompleted.IsUndefined)
        {
            IsCompleted = isCompleted.Value;
        }

        UpdateTimesOfStay(arrivalTime, departureTime, breaks);
    }

    private void UpdateTimesOfStay(ValueOrUndefined<DateTime?> arrivalTime, ValueOrUndefined<DateTime?> departureTime,
        IEnumerable<ObservationBreak>? breaks)

    {
        if (!arrivalTime.IsUndefined)
        {
            ArrivalTime = arrivalTime.Value;
        }

        if (!departureTime.IsUndefined)
        {
            DepartureTime = departureTime.Value;
        }

        if (ArrivalTime.HasValue && DepartureTime.HasValue && DepartureTime >= ArrivalTime)
        {
            MinutesMonitoring = (DepartureTime.Value - ArrivalTime.Value).TotalMinutes;
        }

        if (breaks is not null)
        {
            Breaks = breaks.ToList().AsReadOnly();
        }
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