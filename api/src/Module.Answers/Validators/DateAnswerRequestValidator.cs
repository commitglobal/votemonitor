using FastEndpoints;
using FluentValidation;
using Module.Answers.Requests;

namespace Module.Answers.Validators;

public class DateAnswerRequestValidator : Validator<DateAnswerRequest>
{
    public DateAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
    }
}
