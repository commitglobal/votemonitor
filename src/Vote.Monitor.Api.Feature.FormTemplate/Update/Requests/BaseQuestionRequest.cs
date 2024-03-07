using System.Text.Json.Serialization;
using PolyJson;

namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

[PolyJsonConverter(distriminatorPropertyName: "$questionType")]

[PolyJsonConverter.SubType(typeof(TextInputQuestionRequest), "textInputQuestion")]
[PolyJsonConverter.SubType(typeof(NumberInputQuestionRequest), "numberInputQuestion")]
[PolyJsonConverter.SubType(typeof(DateInputQuestionRequest), "dateInputQuestion")]
[PolyJsonConverter.SubType(typeof(SingleSelectQuestionRequest), "singleSelectQuestion")]
[PolyJsonConverter.SubType(typeof(MultiSelectQuestionRequest), "multiSelectQuestion")]
[PolyJsonConverter.SubType(typeof(RatingQuestionRequest), "ratingQuestion")]
public abstract class BaseQuestionRequest
{
    [JsonPropertyName("$questionType")]
    public string QuestionType => DiscriminatorValue.Get(GetType())!;

    public Guid Id { get; set; }

    public string Code { get; set; }

    public TranslatedString Text { get; set; }

    public TranslatedString? Helptext { get; set; }
}
