using System.Text.Json.Serialization;
using FluentValidation.Results;
using PolyJson;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

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

    public Guid QuestionId { get; private set; }
    
    [JsonConstructor]
    protected BaseAnswer(Guid questionId)
    {
        QuestionId = questionId;
    }

    internal abstract ValidationResult Validate(BaseQuestion question, int index);

    protected static ValidationResult GetInvalidAnswerTypeError(int answerIndex, BaseQuestion question, BaseAnswer answer)
    {
        var propertyName = $"answers[{answerIndex}].QuestionId";
        const string message = "Invalid answer type provided";
        var validationFailure = new ValidationFailure(propertyName, message, answer.Discriminator);

        return new ValidationResult([validationFailure]);
    }
}
