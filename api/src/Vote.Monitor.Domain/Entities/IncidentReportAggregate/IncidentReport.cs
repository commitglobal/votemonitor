using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.IncidentReportAttachmentAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportNoteAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.IncidentReportAggregate;

public class IncidentReport : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }

    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }

    public IncidentReportLocationType LocationType { get; private set; }
    public string? LocationDescription { get; private set; }
    public Guid? PollingStationId { get; private set; }
    public PollingStation? PollingStation { get; private set; }
    public Guid FormId { get; private set; }
    public Form Form { get; private set; }
    public int NumberOfQuestionsAnswered { get; private set; }
    public int NumberOfFlaggedAnswers { get; private set; }
    public IncidentReportFollowUpStatus FollowUpStatus { get; private set; }
    public IReadOnlyList<BaseAnswer> Answers { get; private set; } = new List<BaseAnswer>().AsReadOnly();
    private readonly List<IncidentReportNote> _notes = new();
    public virtual IReadOnlyList<IncidentReportNote> Notes => _notes.ToList().AsReadOnly();

    private readonly List<IncidentReportAttachment> _attachments = new();
    public virtual IReadOnlyList<IncidentReportAttachment> Attachments => _attachments.ToList().AsReadOnly();

    public bool IsCompleted { get; private set; }

    private IncidentReport(
        Guid incidentReportId,
        Guid electionRoundId,
        Guid monitoringObserverId,
        IncidentReportLocationType locationType,
        Guid? pollingStationId,
        string? locationDescription,
        Guid formId,
        List<BaseAnswer> answers,
        int numberOfQuestionsAnswered,
        int numberOfFlaggedAnswers,
        bool? isCompleted)
    {
        Id = incidentReportId;
        ElectionRoundId = electionRoundId;
        PollingStationId = pollingStationId;
        MonitoringObserverId = monitoringObserverId;
        FormId = formId;
        Answers = answers.ToList().AsReadOnly();
        NumberOfQuestionsAnswered = numberOfQuestionsAnswered;
        NumberOfFlaggedAnswers = numberOfFlaggedAnswers;
        LocationType = locationType;
        LocationDescription = locationDescription;
        FollowUpStatus = IncidentReportFollowUpStatus.NotApplicable;
        if(isCompleted.HasValue)
        {
            IsCompleted = isCompleted.Value;
        }
    }

    internal static IncidentReport Create(
        Guid incidentReportId,
        Guid electionRoundId,
        MonitoringObserver monitoringObserver,
        IncidentReportLocationType locationType,
        Guid? pollingStationId,
        string? locationDescription,
        Guid formId,
        List<BaseAnswer> answers,
        int numberOfQuestionAnswered,
        int numberOfFlaggedAnswers,
        bool? isCompleted)
    {
        if (locationType == IncidentReportLocationType.PollingStation)
        {
            if (pollingStationId == null)
            {
                throw new ArgumentNullException(nameof(pollingStationId),
                    "PollingStation is mandatory when location type is PollingStation");
            }

            return new(incidentReportId,
                electionRoundId,
                monitoringObserver.Id,
                IncidentReportLocationType.PollingStation,
                pollingStationId,
                null,
                formId,
                answers,
                numberOfQuestionAnswered,
                numberOfFlaggedAnswers,
                isCompleted);
        }

        if (locationType == IncidentReportLocationType.OtherLocation)
        {
            if (string.IsNullOrWhiteSpace(locationDescription))
            {
                throw new ArgumentNullException(nameof(locationDescription),
                    "LocationDescription is mandatory when location type is OtherLocation");
            }

            return new(incidentReportId,
                electionRoundId,
                monitoringObserver.Id,
                IncidentReportLocationType.OtherLocation,
                null,
                locationDescription,
                formId,
                answers,
                numberOfQuestionAnswered,
                numberOfFlaggedAnswers,
                isCompleted);
        }

        throw new ArgumentNullException(nameof(locationType),
            $"Unknown location type '{locationType}");
    }
    
    internal void Update(IEnumerable<BaseAnswer>? answers,
        int? numberOfQuestionsAnswered,
        int? numberOfFlaggedAnswers,
        bool? isCompleted)
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
        
        if(isCompleted.HasValue)
        {
            IsCompleted = isCompleted.Value;
        }
    }

    public void ClearAnswers()
    {
        Answers = [];
        NumberOfFlaggedAnswers = 0;
        NumberOfQuestionsAnswered = 0;
    }

    public void UpdateFollowUpStatus(IncidentReportFollowUpStatus followUpStatus)
    {
        FollowUpStatus = followUpStatus;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private IncidentReport()
    {
    }
#pragma warning restore CS8618
}