using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

public class PollingStationInformation : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; internal set; }    
    public Guid ElectionRoundId { get; internal set; }
    public ElectionRound ElectionRound { get; internal set; }
    public Guid PollingStationId { get; internal set; }
    public PollingStation PollingStation { get; internal set; }
    public Guid MonitoringObserverId { get; internal set; }
    public MonitoringObserver MonitoringObserver { get; internal set; }
    public Guid PollingStationInformationFormId { get; internal set; }
    public PollingStationInformationForm PollingStationInformationForm { get; internal set; }
    public DateTime? ArrivalTime { get; internal set; }
    public DateTime? DepartureTime { get; internal set; }
    public int NumberOfQuestionsAnswered { get; internal set; }
    public int NumberOfFlaggedAnswers { get; internal set; }
    public SubmissionFollowUpStatus FollowUpStatus { get; internal set; }
    public IReadOnlyList<BaseAnswer> Answers { get; internal set; } = new List<BaseAnswer>().AsReadOnly();
    public IReadOnlyList<ObservationBreak> Breaks { get; internal set; } = new List<ObservationBreak>().AsReadOnly();
    public bool IsCompleted { get; internal set; }

    internal PollingStationInformation(
        Guid userId,
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
        ValueOrUndefined<bool> isCompleted)
    {
        Id = Guid.NewGuid();
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
        CreatedBy = userId;
        CreatedOn = DateTime.UtcNow;
        Answers = answers.ToList().AsReadOnly();

        if (!isCompleted.IsUndefined)
        {
            IsCompleted = isCompleted.Value;
        }

        UpdateTimesOfStay(arrivalTime, departureTime, breaks);
    }

    internal static PollingStationInformation Create(
        Guid userId,
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
        new(userId,
            electionRound,
            pollingStation,
            monitoringObserver,
            pollingStationInformationForm,
            arrivalTime,
            departureTime,
            answers,
            numberOfQuestionsAnswered,
            numberOfFlaggedAnswers,
            breaks,
            isCompleted);

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
        
        LastModifiedOn = DateTime.UtcNow;
        
        UpdateTimesOfStay(arrivalTime, departureTime, breaks);
    }

    internal void UpdateTimesOfStay(ValueOrUndefined<DateTime?> arrivalTime, ValueOrUndefined<DateTime?> departureTime,
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

    internal PollingStationInformation()
    {
    }
#pragma warning restore CS8618
}
