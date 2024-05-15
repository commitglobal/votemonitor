using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public record RatingQuestion : BaseQuestion
{
    [JsonConverter(typeof(SmartEnumNameConverter<RatingScale, string>))]
    public RatingScale Scale { get; private set; }

    [JsonConstructor]
    internal RatingQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        RatingScale scale,
        DisplayLogic? displayLogic) : base(id, code, text, helptext, displayLogic)
    {
        Scale = scale;
    }

    public static RatingQuestion Create(Guid id,
        string code,
        TranslatedString text,
        RatingScale scale,
        TranslatedString? helptext = null,
        DisplayLogic? displayLogic = null) =>
        new(id, code, text, helptext, scale, displayLogic);

    protected override void AddTranslationsInternal(string languageCode) { }
    protected override void RemoveTranslationInternal(string languageCode) { }
}
