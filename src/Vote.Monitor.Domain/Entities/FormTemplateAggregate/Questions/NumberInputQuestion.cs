using System.Text.Json.Serialization;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

public class NumberInputQuestion : BaseQuestion
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public TranslatedString? InputPlaceholder { get; private set; }

    [JsonConstructor]
    internal NumberInputQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        TranslatedString? inputPlaceholder) : base(text, helptext)
    {
        Id = id;
        Code = code;
        InputPlaceholder = inputPlaceholder;
    }

    public static NumberInputQuestion Create(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        TranslatedString? inputPlaceholder)
        => new(id, code, text, helptext, inputPlaceholder);
}
