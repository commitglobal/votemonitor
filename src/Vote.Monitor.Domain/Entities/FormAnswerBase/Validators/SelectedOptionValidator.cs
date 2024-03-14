using FluentValidation;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;

public class SelectedOptionValidator : AbstractValidator<SelectedOption>
{
    public SelectedOptionValidator(IReadOnlyList<SelectOption> questionOptions)
    {
        RuleFor(x => x.Text).MaximumLength(1024);

        RuleFor(x => x.OptionId)
            .Must(optionId => questionOptions.Any(x => x.Id == optionId));
    }
}
