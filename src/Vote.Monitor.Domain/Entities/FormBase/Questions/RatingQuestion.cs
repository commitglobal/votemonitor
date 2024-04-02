using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.FormBase.Questions;

public class RatingQuestion : BaseQuestion
{
    public string Code { get; private set; }

    [JsonConverter(typeof(SmartEnumNameConverter<RatingScale, string>))]
    public RatingScale Scale { get; private set; }

    [JsonConstructor]
    internal RatingQuestion(Guid id,
        string code,
        TranslatedString text,
        TranslatedString? helptext,
        RatingScale scale) : base(id, text, helptext)
    {
        Code = code;
        Scale = scale;
    }

    public static RatingQuestion Create(Guid id, string code, TranslatedString text, TranslatedString? helptext, RatingScale scale) =>
        new(id, code, text, helptext, scale);
}
