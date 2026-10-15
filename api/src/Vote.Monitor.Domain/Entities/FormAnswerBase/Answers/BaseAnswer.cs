using FluentValidation.Results;
using PolyJson;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

[PolyJsonConverter(distriminatorPropertyName: "$answerType")]

[PolyJsonConverter.SubType(typeof(TextAnswer), AnswerTypes.TextAnswerType)]
[PolyJsonConverter.SubType(typeof(NumberAnswer), AnswerTypes.NumberAnswerType)]
[PolyJsonConverter.SubType(typeof(DateAnswer), AnswerTypes.DateAnswerType)]
[PolyJsonConverter.SubType(typeof(TimeAnswer), AnswerTypes.TimeAnswerType)]
[PolyJsonConverter.SubType(typeof(DateTimeAnswer), AnswerTypes.DateTimeAnswerType)]
[PolyJsonConverter.SubType(typeof(SingleSelectAnswer), AnswerTypes.SingleSelectAnswerType)]
[PolyJsonConverter.SubType(typeof(MultiSelectAnswer), AnswerTypes.MultiSelectAnswerType)]
[PolyJsonConverter.SubType(typeof(RatingAnswer), AnswerTypes.RatingAnswerType)]
public abstract record BaseAnswer
{
    [JsonPropertyName("$answerType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual string Discriminator => DiscriminatorValue.Get(GetType());

    public Guid QuestionId { get; private set; }

    [JsonInclude]
    public DateTime? CreatedAt { get; private set; }

    [JsonInclude]
    public DateTime? LastUpdatedAt { get; private set; }

    [JsonConstructor]
    protected BaseAnswer(Guid questionId)
    {
        QuestionId = questionId;
    }

    public BaseAnswer WithTimestamps(DateTime? createdAt, DateTime? lastUpdatedAt) =>
        this with { CreatedAt = createdAt, LastUpdatedAt = lastUpdatedAt };

    public abstract ValidationResult Validate(BaseQuestion question, int index);

    protected static ValidationResult GetInvalidAnswerTypeError(int answerIndex, BaseQuestion question, BaseAnswer answer)
    {
        var propertyName = $"answers[{answerIndex}].QuestionId";
        string message = $"Invalid answer type '{answer.Discriminator}' provided for question of type '{question.Discriminator}'";
        var validationFailure = new ValidationFailure(propertyName, message, answer.Discriminator);

        return new ValidationResult([validationFailure]);
    }
}
