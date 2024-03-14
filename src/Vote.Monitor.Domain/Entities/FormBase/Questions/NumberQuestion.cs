using System.Text.Json.Serialization;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public class NumberQuestion : BaseQuestion
{
    public string Code { get; private set; }
    public TranslatedString? InputPlaceholder { get; private set; }

    [JsonConstructor]
    internal NumberQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        TranslatedString? inputPlaceholder) : base(id, text, helptext)
    {
        Code = code;
        InputPlaceholder = inputPlaceholder;
    }

    public static NumberQuestion Create(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        TranslatedString? inputPlaceholder)
        => new(id, code, text, helptext, inputPlaceholder);
}
