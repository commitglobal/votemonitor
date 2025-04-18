using System.Text.Json.Serialization;
using PolyJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Forms.Requests;

[PolyJsonConverter(distriminatorPropertyName: "$questionType")]
[PolyJsonConverter.SubType(typeof(TextQuestionRequest), QuestionTypes.TextQuestionType)]
[PolyJsonConverter.SubType(typeof(NumberQuestionRequest), QuestionTypes.NumberQuestionType)]
[PolyJsonConverter.SubType(typeof(DateQuestionRequest), QuestionTypes.DateQuestionType)]
[PolyJsonConverter.SubType(typeof(SingleSelectQuestionRequest), QuestionTypes.SingleSelectQuestionType)]
[PolyJsonConverter.SubType(typeof(MultiSelectQuestionRequest), QuestionTypes.MultiSelectQuestionType)]
[PolyJsonConverter.SubType(typeof(RatingQuestionRequest), QuestionTypes.RatingQuestionType)]
public abstract class BaseQuestionRequest
{
    [JsonPropertyName("$questionType")] public string QuestionType => DiscriminatorValue.Get(GetType())!;

    public Guid Id { get; set; }

    public string Code { get; set; }

    public TranslatedString Text { get; set; }

    public TranslatedString? Helptext { get; set; }
    public DisplayLogicRequest? DisplayLogic { get; set; }
}