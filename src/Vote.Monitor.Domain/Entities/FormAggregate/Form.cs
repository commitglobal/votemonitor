namespace Vote.Monitor.Domain.Entities.FormAggregate;

public class Form : IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    private Form()
    {
    }

    public Form(Guid electionRoundId, Guid languageId, string code, string description)
    {
        ElectionRoundId = electionRoundId;
        Code = code;
        LanguageId = languageId;
        Description = description;
        Status = FormStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public Guid LanguageId { get; private set; }
    public string Code { get; private set; }
    public string Description { get; private set; }
    public FormStatus Status { get; private set; } = FormStatus.Draft;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public List<BaseQuestion> Questions { get; private set; } = new();
    
    public void UpdateDetails(Guid languageId, string code, string description, List<BaseQuestion> questions)
    {
        LanguageId = languageId;
        Code = code;
        Description = description;
        Questions = questions;
        UpdatedAt = DateTime.UtcNow;
    }
}
