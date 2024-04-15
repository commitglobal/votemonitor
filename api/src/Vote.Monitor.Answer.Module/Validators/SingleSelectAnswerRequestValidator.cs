using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Answer.Module.Requests;

namespace Vote.Monitor.Answer.Module.Validators;

public class SingleSelectAnswerRequestValidator : Validator<SingleSelectAnswerRequest>
{
    public SingleSelectAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Selection).SetValidator(new SelectedOptionRequestValidator());
    }
}
