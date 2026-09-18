using FastEndpoints;
using FluentValidation;
using Module.Forms.Requests;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Forms.Validators;

public class DisplayLogicRequestValidator : Validator<DisplayLogicRequest>
{
    public DisplayLogicRequestValidator()
    {
        RuleFor(x => x.ParentQuestionId).NotEmpty();
        RuleFor(x => x.Condition).NotEmpty();

        RuleFor(x => x.Value)
            .NotEmpty()
            .MaximumLength(1024)
            .When(x => x.Condition != DisplayLogicCondition.AnyOf && x.Condition != DisplayLogicCondition.All);

        RuleFor(x => x.OptionIds)
            .NotEmpty()
            .When(x => x.Condition == DisplayLogicCondition.AnyOf || x.Condition == DisplayLogicCondition.All);

        RuleForEach(x => x.OptionIds)
            .NotEmpty()
            .When(x => x.OptionIds is not null);
    }
}
