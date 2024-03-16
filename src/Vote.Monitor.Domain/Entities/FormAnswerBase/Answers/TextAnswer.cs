using System.Text.Json.Serialization;
using FluentValidation.Results;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

public class TextAnswer : BaseAnswer
{
    public string Text { get; private set; }

    [JsonConstructor]
    internal TextAnswer(Guid questionId, string text) : base(questionId)
    {
        Text = text;
    }

    public static TextAnswer Create(Guid questionId, string text) => new(questionId, text);

    public override ValidationResult Validate(BaseQuestion question, int index)
    {
        if (question.GetType() != typeof(DateQuestion))
        {
            return GetInvalidAnswerTypeError(index, question, this);
        }

        return new TextAnswerValidator().Validate(this);
    }
}
