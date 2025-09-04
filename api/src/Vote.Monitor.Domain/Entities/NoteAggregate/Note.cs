using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.NoteAggregate;

public class Note : IAggregateRoot
{
    public Guid Id { get; private set; }

    [Obsolete("Will be removed in future version")]
    public Guid ElectionRoundId { get; private set; }

    [Obsolete("Will be removed in future version")]
    public Guid PollingStationId { get; private set; }

    [Obsolete("Will be removed in future version")]
    public Guid FormId { get; private set; }
    
    public Guid MonitoringObserverId { get; private set; }

    public Guid SubmissionId { get; private set; }
    public Guid QuestionId { get; private set; }
    public string Text { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }
    
    public MonitoringObserver MonitoringObserver { get; private set; }

    public Note(Guid id,
        Guid electionRoundId,
        Guid pollingStationId,
        Guid monitoringObserverId,
        Guid formId,
        Guid questionId,
        string text,
        DateTime lastUpdatedAt)
    {
        Id = id;
        ElectionRoundId = electionRoundId;
        PollingStationId = pollingStationId;
        MonitoringObserverId = monitoringObserverId;
        FormId = formId;
        QuestionId = questionId;
        Text = text;
        LastUpdatedAt = lastUpdatedAt;
    }

    private Note(Guid id,
        Guid monitoringObserverId,
        Guid questionId,
        string text,
        DateTime lastUpdatedAt)
    {
        Id = id;
        MonitoringObserverId = monitoringObserverId;
        QuestionId = questionId;
        Text = text;
        LastUpdatedAt = lastUpdatedAt;
    }

    public static Note Create(Guid id,
        Guid monitoringObserverId,
        Guid submissionId,
        Guid questionId,
        string text,
        DateTime lastUpdatedAt) => new(id, monitoringObserverId, questionId, text, lastUpdatedAt);

    public void UpdateText(string text, DateTime lastUpdatedAt)
    {
        Text = text;
        LastUpdatedAt = lastUpdatedAt;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal Note()
    {
    }
#pragma warning restore CS8618
}
