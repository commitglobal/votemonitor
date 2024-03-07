using System.Text.Json.Serialization;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate;

public class FormSection
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public TranslatedString Title { get; private set; }
    public IReadOnlyList<BaseQuestion> Questions { get; private set; }

    public FormSection()
    {

    }

    [JsonConstructor]
    public FormSection(Guid id, string code, TranslatedString title, IReadOnlyList<BaseQuestion> questions)
    {
        Id = id;
        Code = code;
        Title = title;
        Questions = questions;
    }

    public static FormSection Create(string code, TranslatedString title, IReadOnlyList<BaseQuestion> questions)
    {
        return new FormSection(Guid.NewGuid(), code, title, questions);
    }
}
