using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Answer.Module.Requests;

namespace Vote.Monitor.Answer.Module.Validators;

public class DateAnswerRequestValidator : Validator<DateAnswerRequest>
{
    public DateAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
    }
}
