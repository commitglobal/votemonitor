using System.Text.Json.Serialization;
using PolyJson;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Form.Module.Models;


[PolyJsonConverter(distriminatorPropertyName: "$questionType")]

[PolyJsonConverter.SubType(typeof(TextQuestionModel), "textInputQuestion")]
[PolyJsonConverter.SubType(typeof(NumberQuestionModel), "numberInputQuestion")]
[PolyJsonConverter.SubType(typeof(DateQuestionModel), "dateInputQuestion")]
[PolyJsonConverter.SubType(typeof(SingleSelectQuestionModel), "singleSelectQuestion")]
[PolyJsonConverter.SubType(typeof(MultiSelectQuestionModel), "multiSelectQuestion")]
[PolyJsonConverter.SubType(typeof(RatingQuestionModel), "ratingQuestion")]
public abstract class BaseQuestionModel
{
    [JsonPropertyName("$questionType")]
    public string Discriminator => DiscriminatorValue.Get(GetType());

    public TranslatedString Text { get; init; }

    public TranslatedString? Helptext { get; init; }
}
