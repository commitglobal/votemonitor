using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Form.Module.Requests;

namespace Vote.Monitor.Form.Module.Validators;

public class DisplayLogicRequestValidator : Validator<DisplayLogicRequest>
{
    public DisplayLogicRequestValidator()
    {
        RuleFor(x => x.ParentQuestionId).NotEmpty();
        RuleFor(x => x.Condition).NotEmpty();
        RuleFor(x => x.Value).NotEmpty().MaximumLength(1024);
    }
}
