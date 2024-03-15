using System.Text.Json.Serialization;
using PolyJson;

namespace Vote.Monitor.Answer.Module.Models;

[PolyJsonConverter(distriminatorPropertyName: "$answerType")]

[PolyJsonConverter.SubType(typeof(TextAnswerModel), "textAnswer")]
[PolyJsonConverter.SubType(typeof(NumberAnswerModel), "numberAnswer")]
[PolyJsonConverter.SubType(typeof(DateAnswerModel), "dateAnswer")]
[PolyJsonConverter.SubType(typeof(SingleSelectAnswerModel), "singleSelectAnswer")]
[PolyJsonConverter.SubType(typeof(MultiSelectAnswerModel), "multiSelectAnswer")]
[PolyJsonConverter.SubType(typeof(RatingAnswerModel), "ratingAnswer")]
public abstract class BaseAnswerModel
{
    [JsonPropertyName("$answerType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Discriminator => DiscriminatorValue.Get(GetType());

    public Guid QuestionId { get; set; }
}
