using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public record RatingQuestion : BaseQuestion
{
    public TranslatedString? LowerLabel { get; private set; }
    public TranslatedString? UpperLabel { get; private set; }
    
    [JsonConverter(typeof(SmartEnumNameConverter<RatingScale, string>))]
    public RatingScale Scale { get; private set; }

    [JsonConstructor]
    internal RatingQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        TranslatedString? lowerLabel,
        TranslatedString? upperLabel,
        RatingScale scale,
        DisplayLogic? displayLogic) : base(id, code, text, helptext, displayLogic)
    {
        LowerLabel = lowerLabel;
        UpperLabel = upperLabel;
        Scale = scale;
    }

    public static RatingQuestion Create(Guid id,
        string code,
        TranslatedString text,
        RatingScale scale,
        TranslatedString? helptext = null,
        TranslatedString? lowerLabel = null,
        TranslatedString? upperLabel = null,
        DisplayLogic? displayLogic = null) =>
        new(id, code, text, helptext, lowerLabel, upperLabel, scale, displayLogic);

    protected override void AddTranslationsInternal(string languageCode)
    {
        LowerLabel?.AddTranslation(languageCode);
        UpperLabel?.AddTranslation(languageCode);
    }

    protected override void RemoveTranslationInternal(string languageCode)
    {
        LowerLabel?.RemoveTranslation(languageCode);
        UpperLabel?.RemoveTranslation(languageCode);
    }
    
    protected override TranslationStatus InternalGetTranslationStatus(string baseLanguageCode, string languageCode)
    {
        if (LowerLabel != null && !string.IsNullOrWhiteSpace(LowerLabel[baseLanguageCode]) &&
            string.IsNullOrWhiteSpace(LowerLabel[languageCode]))
        {
            return TranslationStatus.MissingTranslations;
        }
        
        if (UpperLabel != null && !string.IsNullOrWhiteSpace(UpperLabel[baseLanguageCode]) &&
            string.IsNullOrWhiteSpace(UpperLabel[languageCode]))
        {
            return TranslationStatus.MissingTranslations;
        }
        
        return TranslationStatus.Translated;
    }
}