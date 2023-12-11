using System.ComponentModel.DataAnnotations.Schema;

namespace Vote.Monitor.Domain.Entities.FormAggregate;

public class Form : IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    private Form()
    {
    }

    public Form(string code, Guid languageId, string description)
    {
        Code = code;
        LanguageId = languageId;
        Description = description;
        Status = FormStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public Guid LanguageId { get; private set; }
    public string Description { get; private set; }
    public FormStatus Status { get; private set; } = FormStatus.Draft;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public List<BaseQuestion> Questions { get; private set; } = new();


    // TBD
    public void UpdateDetails()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
