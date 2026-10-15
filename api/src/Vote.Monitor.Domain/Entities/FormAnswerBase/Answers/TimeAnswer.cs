using FluentValidation.Results;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

public record TimeAnswer : BaseAnswer
{
    public TimeOnly Time { get; private set; }

    [JsonConstructor]
    internal TimeAnswer(Guid questionId, TimeOnly time) : base(questionId)
    {
        Time = time;
    }
    public static TimeAnswer Create(Guid questionId, TimeOnly time) => new(questionId, time);

    public override ValidationResult Validate(BaseQuestion question, int index)
    {
        if (question.GetType() != typeof(TimeQuestion))
        {
            return GetInvalidAnswerTypeError(index, question, this);
        }

        return new TimeAnswerValidator().Validate(this);
    }
}
