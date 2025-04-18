using FastEndpoints;
using FluentValidation;
using Module.Forms.Requests;

namespace Module.Forms.Validators;

public class DisplayLogicRequestValidator : Validator<DisplayLogicRequest>
{
    public DisplayLogicRequestValidator()
    {
        RuleFor(x => x.ParentQuestionId).NotEmpty();
        RuleFor(x => x.Condition).NotEmpty();
        RuleFor(x => x.Value).NotEmpty().MaximumLength(1024);
    }
}
