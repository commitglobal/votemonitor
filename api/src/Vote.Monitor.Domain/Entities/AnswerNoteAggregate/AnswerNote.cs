using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.AnswerNoteAggregate;

public class AnswerNote : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid FormId { get; private set; }
    public Form Form { get; private set; }
    public Guid QuestionId { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }
    public string Text { get; private set; }

    internal AnswerNote(ElectionRound electionRound,
        Form form,
        Guid questionId,
        MonitoringObserver monitoringObserver,
        string text) : base(Guid.NewGuid())
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        Form = form;
        FormId = form.Id;
        QuestionId = questionId;
        MonitoringObserver = monitoringObserver;
        MonitoringObserverId = monitoringObserver.Id;
        Text = text;
    }

    public void Update(string text)
    {
        Text = text;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal AnswerNote()
    {
    }
#pragma warning restore CS8618
}
