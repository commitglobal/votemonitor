using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public record TextQuestion : BaseQuestion
{
    public TranslatedString? InputPlaceholder { get; private set; }

    [JsonConstructor]
    internal TextQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        TranslatedString? inputPlaceholder,
        DisplayLogic? displayLogic) : base(id, code, text, helptext, displayLogic)
    {
        InputPlaceholder = inputPlaceholder;
    }

    public static TextQuestion Create(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext = null,
        TranslatedString? inputPlaceholder = null,
        DisplayLogic? displayLogic = null)
        => new(id, code, text, helptext, inputPlaceholder, displayLogic);

    protected override void AddTranslationsInternal(string languageCode)
    {
        InputPlaceholder?.AddTranslation(languageCode);
    }
    
    protected override void RemoveTranslationInternal(string languageCode)
    {
        InputPlaceholder?.RemoveTranslation(languageCode);
    }
    
    protected override TranslationStatus InternalGetTranslationStatus(string baseLanguageCode, string languageCode)
    {
        if (InputPlaceholder != null && !string.IsNullOrWhiteSpace(InputPlaceholder[baseLanguageCode]) &&
            string.IsNullOrWhiteSpace(InputPlaceholder[languageCode]))
        {
            return TranslationStatus.MissingTranslations;
        }
        
        return TranslationStatus.Translated;
    }
}
