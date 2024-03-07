using System.Text.Json.Serialization;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

public class MultiSelectQuestion : BaseQuestion
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public IReadOnlyList<SelectOption> Options { get; private set; }

    [JsonConstructor]
    private MultiSelectQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        IReadOnlyList<SelectOption> options) : base(text, helptext)
    {
        Id = id;
        Code = code;
        Options = options.ToList().AsReadOnly();
    }

    public static MultiSelectQuestion Create(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        IReadOnlyList<SelectOption> options)
        => new(id, code, text, helptext, options);
}
