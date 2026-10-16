using FastEndpoints;
using FluentValidation;
using Module.Answers.Requests;

namespace Module.Answers.Validators;

public class TimeAnswerRequestValidator : Validator<TimeAnswerRequest>
{
    public TimeAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
    }
}
