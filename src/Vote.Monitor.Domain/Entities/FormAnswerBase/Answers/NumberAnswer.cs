using System.Text.Json.Serialization;
using FluentValidation.Results;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

public class NumberAnswer : BaseAnswer
{
    public int Value { get; private set; }

    [JsonConstructor]
    internal NumberAnswer(Guid questionId, int value) : base(questionId)
    {
        Value = value;
    }

    public static NumberAnswer Create(Guid questionId, int value) => new(questionId, value);

    public override ValidationResult Validate(BaseQuestion question, int index)
    {
        if (question.GetType() != typeof(NumberQuestion))
        {
            return GetInvalidAnswerTypeError(index, question, this);
        }

        return new NumberAnswerValidator().Validate(this);
    }
}
