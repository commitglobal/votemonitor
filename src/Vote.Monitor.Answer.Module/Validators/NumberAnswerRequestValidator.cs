using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Answer.Module.Requests;

namespace Vote.Monitor.Answer.Module.Validators;

public class NumberAnswerRequestValidator : Validator<NumberAnswerRequest>
{
    public NumberAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Value).GreaterThanOrEqualTo(0);
    }
}
