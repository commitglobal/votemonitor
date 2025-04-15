using FastEndpoints;
using FluentValidation;
using Module.Answers.Requests;

namespace Module.Answers.Validators;

public class SelectedOptionRequestValidator : Validator<SelectedOptionRequest>
{
    public SelectedOptionRequestValidator()
    {
        RuleFor(x => x.OptionId).NotEmpty();
        RuleFor(x => x.Text).MaximumLength(1024);
    }
}
