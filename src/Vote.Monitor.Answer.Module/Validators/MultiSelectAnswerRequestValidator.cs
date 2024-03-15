using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Answer.Module.Requests;

namespace Vote.Monitor.Answer.Module.Validators;

public class MultiSelectAnswerRequestValidator : Validator<MultiSelectAnswerRequest>
{
    public MultiSelectAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Selection).NotEmpty();
        RuleForEach(x => x.Selection).SetValidator(new SelectedOptionRequestValidator());
    }
}
