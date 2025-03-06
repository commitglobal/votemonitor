using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.NoteAggregate;

public class Note : IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public Guid PollingStationId { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public Guid FormId { get; private set; }
    public Guid QuestionId { get; private set; }
    public string Text { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    public ElectionRound ElectionRound { get; private set; }
    public Form Form { get; private set; }
    public PollingStation PollingStation { get; private set; }
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
