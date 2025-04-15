using System.Text.Json.Serialization;
using PolyJson;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Module.Answers.Models;

[PolyJsonConverter(distriminatorPropertyName: "$answerType")]

[PolyJsonConverter.SubType(typeof(TextAnswerModel), AnswerTypes.TextAnswerType)]
[PolyJsonConverter.SubType(typeof(NumberAnswerModel), AnswerTypes.NumberAnswerType)]
[PolyJsonConverter.SubType(typeof(DateAnswerModel), AnswerTypes.DateAnswerType)]
[PolyJsonConverter.SubType(typeof(SingleSelectAnswerModel), AnswerTypes.SingleSelectAnswerType)]
[PolyJsonConverter.SubType(typeof(MultiSelectAnswerModel), AnswerTypes.MultiSelectAnswerType)]
[PolyJsonConverter.SubType(typeof(RatingAnswerModel), AnswerTypes.RatingAnswerType)]
public abstract class BaseAnswerModel
{
    [JsonPropertyName("$answerType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Discriminator => DiscriminatorValue.Get(GetType());

    public Guid QuestionId { get; set; }
}
