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
        Options = options;
    }

    public SelectOption AddOption(Guid id, TranslatedString text, bool isFreeText, bool isFlagged)
    {
        var option = SelectOption.Create(id, text, isFreeText, isFlagged);
        Options = Options.Concat([option]).ToList().AsReadOnly();

        return option;
    }

    public static MultiSelectQuestion Create(
        Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext)
        => new(id, code, text, helptext, []);
}
