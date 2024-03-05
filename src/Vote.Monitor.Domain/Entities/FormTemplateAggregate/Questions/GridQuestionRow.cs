using System.Text.Json.Serialization;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

public class GridQuestionRow
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public TranslatedString Text { get; private set; }
    public TranslatedString? Helptext { get; private set; }

    [JsonConstructor]
    internal GridQuestionRow(Guid id, string code, TranslatedString text, TranslatedString? helptext)
    {
        Id = id;
        Code = code;
        Text = text;
        Helptext = helptext;
    }

    public static GridQuestionRow Create(Guid id, string code, TranslatedString text, TranslatedString? helptext) =>
        new (id, code, text, helptext);
}
