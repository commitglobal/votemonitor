using FastEndpoints;
using FluentValidation;
using Module.Answers.Requests;

namespace Module.Answers.Validators;

public class MultiSelectAnswerRequestValidator : Validator<MultiSelectAnswerRequest>
{
    public MultiSelectAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleForEach(x => x.Selection).SetValidator(new SelectedOptionRequestValidator());
    }
}
