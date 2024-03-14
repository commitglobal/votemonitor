using FluentValidation;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;

public class MultiSelectAnswerValidator : AbstractValidator<MultiSelectAnswer>
{
    public MultiSelectAnswerValidator(IReadOnlyList<SelectOption> questionOptions)
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleForEach(x => x.Selection).SetValidator(new SelectedOptionValidator(questionOptions));
    }
}
