using System.Text.Json.Serialization;
using FluentValidation.Results;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

public record MultiSelectAnswer : BaseAnswer
{
    public IReadOnlyList<SelectedOption> Selection { get; private set; } = [];

    [JsonConstructor]
    internal MultiSelectAnswer(Guid questionId, IReadOnlyList<SelectedOption> selection) : base(questionId)
    {
        Selection = selection;
    }

    public static MultiSelectAnswer Create(Guid questionId, IReadOnlyList<SelectedOption> selection) =>
        new(questionId, selection);

    public override ValidationResult Validate(BaseQuestion question, int index)
    {
        if (question.GetType() != typeof(MultiSelectQuestion))
        {
            return GetInvalidAnswerTypeError(index, question, this);
        }

        var multiSelectQuestion = question as MultiSelectQuestion;
        return new MultiSelectAnswerValidator(multiSelectQuestion!).Validate(this);
    }

    public virtual bool Equals(MultiSelectAnswer? other)
    {
        return base.Equals(other) && Selection.SequenceEqual(other.Selection);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Selection);
    }
}
