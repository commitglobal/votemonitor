using FluentValidation.Results;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

public record RatingAnswer : BaseAnswer
{
    public int Value { get; private set; }

    [JsonConstructor]
    internal RatingAnswer(Guid questionId, int value) : base(questionId)
    {
        Value = value;
    }

    public static RatingAnswer Create(Guid questionId, int value) => new(questionId, value);

    public override ValidationResult Validate(BaseQuestion question, int index)
    {
        if (question.GetType() != typeof(RatingQuestion))
        {
            return GetInvalidAnswerTypeError(index, question, this);
        }

        var ratingQuestion = question as RatingQuestion;
        return new RatingAnswerValidator(ratingQuestion!).Validate(this);
    }
}
