using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.IssueReportAttachmentAggregate;
using Vote.Monitor.Domain.Entities.IssueReportNoteAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.IssueReportAggregate;

public class IssueReport : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }

    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }

    public IssueReportLocationType LocationType { get; private set; }
    public string? LocationDescription { get; private set; }
    public Guid? PollingStationId { get; private set; }
    public PollingStation? PollingStation { get; private set; }
    public Guid FormId { get; private set; }
    public Form Form { get; private set; }
    public int NumberOfQuestionsAnswered { get; private set; }
    public int NumberOfFlaggedAnswers { get; private set; }
    public IssueReportFollowUpStatus FollowUpStatus { get; private set; }
    public IReadOnlyList<BaseAnswer> Answers { get; private set; } = new List<BaseAnswer>().AsReadOnly();
    private readonly List<IssueReportNote> _notes = new();
    public virtual IReadOnlyList<IssueReportNote> Notes => _notes.ToList().AsReadOnly();

    private readonly List<IssueReportAttachment> _attachments = new();
    public virtual IReadOnlyList<IssueReportAttachment> Attachments => _attachments.ToList().AsReadOnly();

    private IssueReport(
        Guid issueReportId,
        Guid electionRoundId,
        Guid monitoringObserverId,
        IssueReportLocationType locationType,
        Guid? pollingStationId,
        string? locationDescription,
        Guid formId,
        List<BaseAnswer> answers,
        int numberOfQuestionsAnswered,
        int numberOfFlaggedAnswers) : base(issueReportId)
    {
        ElectionRoundId = electionRoundId;
        PollingStationId = pollingStationId;
        MonitoringObserverId = monitoringObserverId;
        FormId = formId;
        Answers = answers.ToList().AsReadOnly();
        NumberOfQuestionsAnswered = numberOfQuestionsAnswered;
        NumberOfFlaggedAnswers = numberOfFlaggedAnswers;
        LocationType = locationType;
        LocationDescription = locationDescription;
        FollowUpStatus = IssueReportFollowUpStatus.NotApplicable;
    }

    internal static IssueReport Create(
        Guid issueReportId,
        Guid electionRoundId,
        MonitoringObserver monitoringObserver,
        IssueReportLocationType locationType,
        Guid? pollingStationId,
        string? locationDescription,
        Guid formId,
        List<BaseAnswer> answers,
        int numberOfQuestionAnswered,
        int numberOfFlaggedAnswers)
    {
        if (locationType == IssueReportLocationType.PollingStation)
        {
            if (pollingStationId == null)
            {
                throw new ArgumentNullException(nameof(pollingStationId),
                    "PollingStation is mandatory when location type is PollingStation");
            }

            return new(issueReportId, electionRoundId, monitoringObserver.Id, IssueReportLocationType.PollingStation,
                pollingStationId, null,
                formId,
                answers, numberOfQuestionAnswered,
                numberOfFlaggedAnswers);
        }

        if (locationType == IssueReportLocationType.OtherLocation)
        {
            if (string.IsNullOrWhiteSpace(locationDescription))
            {
                throw new ArgumentNullException(nameof(locationDescription),
                    "LocationDescription is mandatory when location type is OtherLocation");
            }

            return new(issueReportId, electionRoundId, monitoringObserver.Id, IssueReportLocationType.OtherLocation,
                null,
                locationDescription,
                formId,
                answers, numberOfQuestionAnswered,
                numberOfFlaggedAnswers);
        }

        throw new ArgumentNullException(nameof(locationType),
            $"Unknown location type '{locationType}");
    }

    internal void UpdateAnswers(IEnumerable<BaseAnswer> answers, int numberOfQuestionsAnswered,
        int numberOfFlaggedAnswers)
    {
        Answers = answers.ToList().AsReadOnly();
        NumberOfFlaggedAnswers = numberOfFlaggedAnswers;
        NumberOfQuestionsAnswered = numberOfQuestionsAnswered;
    }

    public void ClearAnswers()
    {
        Answers = [];
        NumberOfFlaggedAnswers = 0;
        NumberOfQuestionsAnswered = 0;
    }

    public void UpdateFollowUpStatus(IssueReportFollowUpStatus followUpStatus)
    {
        FollowUpStatus = followUpStatus;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private IssueReport()
    {
    }
#pragma warning restore CS8618
}