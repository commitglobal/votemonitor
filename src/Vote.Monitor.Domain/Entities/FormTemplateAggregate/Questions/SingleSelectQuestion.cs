using System.Text.Json.Serialization;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

public class SingleSelectQuestion : BaseQuestion
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public IReadOnlyList<SelectOption> Options { get; private set; }

    [JsonConstructor]
    internal SingleSelectQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        IReadOnlyList<SelectOption> options) : base(text, helptext)
    {
        Id = id;
        Code = code;
        Options = options.ToList().AsReadOnly();
    }

    public static SingleSelectQuestion Create(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        IReadOnlyList<SelectOption> options) =>
        new(id, code, text, helptext, options);
}
