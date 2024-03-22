using System.Text.Json.Serialization;
using PolyJson;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Answer.Module.Requests;

[PolyJsonConverter(distriminatorPropertyName: "$answerType")]

[PolyJsonConverter.SubType(typeof(TextAnswerRequest), AnswerTypes.TextAnswerType)]
[PolyJsonConverter.SubType(typeof(NumberAnswerRequest), AnswerTypes.NumberAnswerType)]
[PolyJsonConverter.SubType(typeof(DateAnswerRequest), AnswerTypes.DateAnswerType)]
[PolyJsonConverter.SubType(typeof(SingleSelectAnswerRequest), AnswerTypes.SingleSelectAnswerType)]
[PolyJsonConverter.SubType(typeof(MultiSelectAnswerRequest), AnswerTypes.MultiSelectAnswerType)]
[PolyJsonConverter.SubType(typeof(RatingAnswerRequest), AnswerTypes.RatingAnswerType)]
public abstract class BaseAnswerRequest
{
    [JsonPropertyName("$answerType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Discriminator => DiscriminatorValue.Get(GetType());

    public Guid QuestionId { get; set; }
}
