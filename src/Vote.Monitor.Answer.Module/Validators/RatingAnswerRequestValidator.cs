using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Answer.Module.Requests;

namespace Vote.Monitor.Answer.Module.Validators;

public class RatingAnswerRequestValidator : Validator<RatingAnswerRequest>
{
    public RatingAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Value).GreaterThanOrEqualTo(0);
    }
}
