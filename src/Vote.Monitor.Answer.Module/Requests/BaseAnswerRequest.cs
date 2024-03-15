using System.Text.Json.Serialization;
using PolyJson;

namespace Vote.Monitor.Answer.Module.Requests;

[PolyJsonConverter(distriminatorPropertyName: "$answerType")]

[PolyJsonConverter.SubType(typeof(TextAnswerRequest), "textAnswer")]
[PolyJsonConverter.SubType(typeof(NumberAnswerRequest), "numberAnswer")]
[PolyJsonConverter.SubType(typeof(DateAnswerRequest), "dateAnswer")]
[PolyJsonConverter.SubType(typeof(SingleSelectAnswerRequest), "singleSelectAnswer")]
[PolyJsonConverter.SubType(typeof(MultiSelectAnswerRequest), "multiSelectAnswer")]
[PolyJsonConverter.SubType(typeof(RatingAnswerRequest), "ratingAnswer")]
public abstract class BaseAnswerRequest
{
    [JsonPropertyName("$answerType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Discriminator => DiscriminatorValue.Get(GetType());

    public Guid QuestionId { get; set; }
}
