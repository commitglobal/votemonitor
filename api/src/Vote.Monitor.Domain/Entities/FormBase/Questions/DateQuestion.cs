using System.Text.Json.Serialization;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public record DateQuestion : BaseQuestion
{
    [JsonConstructor]
    internal DateQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        DisplayLogic? displayLogic) : base(id, code, text, helptext, displayLogic)
    {
    }

    public static DateQuestion Create(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext = null,
        DisplayLogic? displayLogic = null)
        => new(id, code, text, helptext, displayLogic);

    protected override void AddTranslationsInternal(string languageCode) { }
    protected override void RemoveTranslationInternal(string languageCode) { }
}
