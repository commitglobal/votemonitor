using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.AnswerAttachmentAggregate;

public class AnswerAttachment : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid FormId { get; private set; }
    public Form Form { get; private set; }
    public Guid QuestionId { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }
    public string Filename { get; private set; }
    public string MimeType { get; private set; }

    internal AnswerAttachment(ElectionRound electionRound,
        Form form,
        Guid questionId,
        MonitoringObserver monitoringObserver,
        string filename,
        string mimeType) : base(Guid.NewGuid())
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        Form = form;
        FormId = form.Id;
        QuestionId = questionId;
        MonitoringObserver = monitoringObserver;
        MonitoringObserverId = monitoringObserver.Id;
        Filename = filename;
        MimeType = mimeType;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal AnswerAttachment()
    {
    }
#pragma warning restore CS8618
}
