using System.Text.Json.Serialization;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

public class DateInputQuestion : BaseQuestion
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }

    [JsonConstructor]
    internal DateInputQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext) : base(text, helptext)
    {
        Id = id;
        Code = code;
    }

    public static DateInputQuestion Create(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext)
        => new(id, code, text, helptext);
}
