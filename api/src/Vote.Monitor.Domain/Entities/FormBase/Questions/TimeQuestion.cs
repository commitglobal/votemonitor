using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public record TimeQuestion : BaseQuestion
{
    [JsonConstructor]
    internal TimeQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        DisplayLogic? displayLogic) : base(id, code, text, helptext, displayLogic)
    {
    }

    public static TimeQuestion Create(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext = null,
        DisplayLogic? displayLogic = null)
        => new(id, code, text, helptext, displayLogic);

    protected override void AddTranslationsInternal(string languageCode) { }
    protected override void RemoveTranslationInternal(string languageCode) { }
    protected override TranslationStatus InternalGetTranslationStatus(string baseLanguageCode, string languageCode)
    {
        return TranslationStatus.Translated;
    }

    protected override void InternalTrimTranslations(IEnumerable<string> languages)
    {
    }
}
