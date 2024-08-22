using System.Text.Json.Serialization;
using PolyJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Form.Module.Models;


[PolyJsonConverter(distriminatorPropertyName: "$questionType")]

[PolyJsonConverter.SubType(typeof(TextQuestionModel), QuestionTypes.TextQuestionType)]
[PolyJsonConverter.SubType(typeof(NumberQuestionModel), QuestionTypes.NumberQuestionType)]
[PolyJsonConverter.SubType(typeof(DateQuestionModel), QuestionTypes.DateQuestionType)]
[PolyJsonConverter.SubType(typeof(SingleSelectQuestionModel), QuestionTypes.SingleSelectQuestionType)]
[PolyJsonConverter.SubType(typeof(MultiSelectQuestionModel), QuestionTypes.MultiSelectQuestionType)]
[PolyJsonConverter.SubType(typeof(RatingQuestionModel), QuestionTypes.RatingQuestionType)]
public abstract class BaseQuestionModel
{
    [JsonPropertyName("$questionType")]
    public string Discriminator => DiscriminatorValue.Get(GetType());

    public Guid Id { get; init; }
    public string Code { get; init; }
    public TranslatedString Text { get; init; }

    public TranslatedString? Helptext { get; init; }
    public DisplayLogicModel? DisplayLogic { get; init; }
}
