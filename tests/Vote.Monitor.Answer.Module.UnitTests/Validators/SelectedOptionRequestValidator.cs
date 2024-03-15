using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Answer.Module.Requests;

namespace Vote.Monitor.Answer.Module.UnitTests.Validators;

public class SelectedOptionRequestValidator : Validator<SelectedOptionRequest>
{
    public SelectedOptionRequestValidator()
    {
        RuleFor(x => x.OptionId).NotEmpty();
        RuleFor(x => x.Text).MaximumLength(1024);
    }
}
