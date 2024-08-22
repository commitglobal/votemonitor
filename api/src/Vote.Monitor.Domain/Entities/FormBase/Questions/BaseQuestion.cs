using System.Text.Json.Serialization;
using PolyJson;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

[PolyJsonConverter(distriminatorPropertyName: "$questionType")]
[PolyJsonConverter.SubType(typeof(TextQuestion), QuestionTypes.TextQuestionType)]
[PolyJsonConverter.SubType(typeof(NumberQuestion), QuestionTypes.NumberQuestionType)]
[PolyJsonConverter.SubType(typeof(DateQuestion), QuestionTypes.DateQuestionType)]
[PolyJsonConverter.SubType(typeof(SingleSelectQuestion), QuestionTypes.SingleSelectQuestionType)]
[PolyJsonConverter.SubType(typeof(MultiSelectQuestion), QuestionTypes.MultiSelectQuestionType)]
[PolyJsonConverter.SubType(typeof(RatingQuestion), QuestionTypes.RatingQuestionType)]
public abstract record BaseQuestion
{
    [JsonPropertyName("$questionType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Discriminator => DiscriminatorValue.Get(GetType());

    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public TranslatedString Text { get; private set; }
    public TranslatedString? Helptext { get; private set; }
    public DisplayLogic? DisplayLogic { get; private set; }

    [JsonConstructor]
    internal BaseQuestion(Guid id, string code, TranslatedString text, TranslatedString? helptext,
        DisplayLogic? displayLogic)
    {
        Id = id;
        Code = code;
        Text = text;
        Helptext = helptext;
        DisplayLogic = displayLogic;
    }

    public void AddTranslation(string languageCode)
    {
        Text.AddTranslation(languageCode);
        Helptext?.AddTranslation(languageCode);

        AddTranslationsInternal(languageCode);
    }

    protected abstract void AddTranslationsInternal(string languageCode);

    public void RemoveTranslation(string languageCode)
    {
        Text.RemoveTranslation(languageCode);
        Helptext?.RemoveTranslation(languageCode);

        RemoveTranslationInternal(languageCode);
    }

    protected abstract void RemoveTranslationInternal(string languageCode);

    public TranslationStatus GetTranslationStatus(string baseLanguageCode, string languageCode)
    {
        if (string.IsNullOrWhiteSpace(Text[languageCode]))
        {
            return TranslationStatus.MissingTranslations;
        }

        if (Helptext != null && !string.IsNullOrWhiteSpace(Helptext[baseLanguageCode]) &&
            string.IsNullOrWhiteSpace(Helptext[languageCode]))
        {
            return TranslationStatus.MissingTranslations;
        }

        return InternalGetTranslationStatus(baseLanguageCode, languageCode);
    }

    protected abstract TranslationStatus InternalGetTranslationStatus(string baseLanguageCode, string languageCode);
}