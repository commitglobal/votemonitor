using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.AnswerNoteAggregate;

public class AnswerNote: BaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid FormId { get; private set; }
    public Form Form { get; private set; }
    public Guid QuestionId { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }
    public string Text { get; private set; }
    public DateTime Timestamp { get; private set; }

    internal AnswerNote(ElectionRound electionRound,
        Form form,
        Guid questionId,
        MonitoringObserver monitoringObserver,
        string text,
        ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        Form = form;
        FormId = form.Id;
        QuestionId = questionId;
        MonitoringObserver = monitoringObserver;
        MonitoringObserverId = monitoringObserver.Id;
        Timestamp = timeProvider.UtcNow;
        Text = text;
    }

    public void Update(string text, ITimeProvider timeProvider)
    {
        Text = text;
        Timestamp = timeProvider.UtcNow;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal AnswerNote()
    {
    }
#pragma warning restore CS8618
}
