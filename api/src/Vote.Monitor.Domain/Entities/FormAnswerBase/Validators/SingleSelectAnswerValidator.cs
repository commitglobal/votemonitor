using FluentValidation;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;

public class SingleSelectAnswerValidator : AbstractValidator<SingleSelectAnswer>
{
    public SingleSelectAnswerValidator(SingleSelectQuestion question)
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Selection).SetValidator(new SelectedOptionValidator(question.Options));
    }
}
