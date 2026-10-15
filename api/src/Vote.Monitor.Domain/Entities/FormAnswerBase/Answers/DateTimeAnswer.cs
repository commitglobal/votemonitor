using FluentValidation.Results;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

public record DateTimeAnswer : BaseAnswer
{
    public DateTime DateTime { get; private set; }

    [JsonConstructor]
    internal DateTimeAnswer(Guid questionId, DateTime dateTime) : base(questionId)
    {
        DateTime = dateTime;
    }
    public static DateTimeAnswer Create(Guid questionId, DateTime dateTime) => new(questionId, dateTime);

    public override ValidationResult Validate(BaseQuestion question, int index)
    {
        if (question.GetType() != typeof(DateTimeQuestion))
        {
            return GetInvalidAnswerTypeError(index, question, this);
        }

        return new DateTimeAnswerValidator().Validate(this);
    }
}
