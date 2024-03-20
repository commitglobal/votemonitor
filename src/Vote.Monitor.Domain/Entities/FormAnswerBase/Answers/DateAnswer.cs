using System.Text.Json.Serialization;
using FluentValidation.Results;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

public class DateAnswer : BaseAnswer
{
    public DateTime Date { get; private set; }

    [JsonConstructor]
    internal DateAnswer(Guid questionId, DateTime date) : base(questionId)
    {
        Date = date;
    }
    public static DateAnswer Create(Guid questionId, DateTime date) => new(questionId, date);

    public override ValidationResult Validate(BaseQuestion question, int index)
    {
        if (question.GetType() != typeof(DateQuestion))
        {
            return GetInvalidAnswerTypeError(index, question, this);
        }

        return new DateAnswerValidator().Validate(this);
    }
}
