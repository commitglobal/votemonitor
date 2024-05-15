using System.Text.Json.Serialization;
using FluentValidation.Results;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

public record SingleSelectAnswer : BaseAnswer
{
    public SelectedOption Selection { get; private set; }

    [JsonConstructor]
    internal SingleSelectAnswer(Guid questionId, SelectedOption selection) : base(questionId)
    {
        Selection = selection;
    }

    public static SingleSelectAnswer Create(Guid questionId, SelectedOption selection) => new(questionId, selection);

    public override ValidationResult Validate(BaseQuestion question, int index)
    {
        if (question.GetType() != typeof(SingleSelectQuestion))
        {
            return GetInvalidAnswerTypeError(index, question, this);
        }

        var singleSelectQuestion = question as SingleSelectQuestion;
        return new SingleSelectAnswerValidator(singleSelectQuestion!).Validate(this);
    }
}
