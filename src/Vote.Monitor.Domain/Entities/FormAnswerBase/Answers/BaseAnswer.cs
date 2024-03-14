using System.Text.Json.Serialization;
using PolyJson;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

[PolyJsonConverter(distriminatorPropertyName: "$answerType")]

[PolyJsonConverter.SubType(typeof(TextAnswer), "textAnswer")]
[PolyJsonConverter.SubType(typeof(NumberAnswer), "numberAnswer")]
[PolyJsonConverter.SubType(typeof(DateAnswer), "dateAnswer")]
[PolyJsonConverter.SubType(typeof(SingleSelectAnswer), "singleSelectAnswer")]
[PolyJsonConverter.SubType(typeof(MultiSelectAnswer), "multiSelectAnswer")]
[PolyJsonConverter.SubType(typeof(RatingAnswer), "ratingAnswer")]
public abstract class BaseAnswer
{
    [JsonPropertyName("$answerType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Discriminator => DiscriminatorValue.Get(GetType());

    public Guid QuestionId { get; set; }
}
