using FastEndpoints;
using FluentValidation;
using Module.Answers.Requests;

namespace Module.Answers.Validators;

public class DateTimeAnswerRequestValidator : Validator<DateTimeAnswerRequest>
{
    public DateTimeAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.DateTime).NotEmpty();
    }
}
