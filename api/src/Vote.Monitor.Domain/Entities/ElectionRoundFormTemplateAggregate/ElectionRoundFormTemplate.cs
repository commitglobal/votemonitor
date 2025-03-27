using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Vote.Monitor.Domain.Entities.ElectionRoundFormTemplateAggregate;

public class ElectionRoundFormTemplate: AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get;  private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid FormTemplateId { get; private set; }
    public FormTemplate FormTemplate { get; private set; }

    internal ElectionRoundFormTemplate(Guid electionRoundId, Guid formTemplateId)
    {
        Id = Guid.NewGuid();
        ElectionRoundId = electionRoundId;
        FormTemplateId = formTemplateId;
    }

    public static ElectionRoundFormTemplate Create(Guid electionRoundId, Guid formTemplateId) =>
        new(electionRoundId, formTemplateId);
    
#pragma warning disable CS8618 // Required by Entity Framework
    private ElectionRoundFormTemplate()
    {

    }
#pragma warning restore CS8618
}
