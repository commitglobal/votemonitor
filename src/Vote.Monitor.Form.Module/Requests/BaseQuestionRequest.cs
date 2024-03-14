using System.Text.Json.Serialization;
using PolyJson;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Form.Module.Requests;

[PolyJsonConverter(distriminatorPropertyName: "$questionType")]

[PolyJsonConverter.SubType(typeof(TextQuestionRequest), "textInputQuestion")]
[PolyJsonConverter.SubType(typeof(NumberQuestionRequest), "numberInputQuestion")]
[PolyJsonConverter.SubType(typeof(DateQuestionRequest), "dateInputQuestion")]
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
