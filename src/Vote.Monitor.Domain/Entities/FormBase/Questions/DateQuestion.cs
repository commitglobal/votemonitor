using System.Text.Json.Serialization;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public class DateQuestion : BaseQuestion
{
    public string Code { get; private set; }

    [JsonConstructor]
    internal DateQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext) : base(id, text, helptext)
    {
        Code = code;
    }

    public static DateQuestion Create(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext)
        => new(id, code, text, helptext);
}
