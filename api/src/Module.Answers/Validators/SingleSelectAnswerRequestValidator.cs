using FastEndpoints;
using FluentValidation;
using Module.Answers.Requests;

namespace Module.Answers.Validators;

public class SingleSelectAnswerRequestValidator : Validator<SingleSelectAnswerRequest>
{
    public SingleSelectAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Selection).SetValidator(new SelectedOptionRequestValidator());
    }
}
