using FluentValidation;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;

public class DateAnswerValidator : AbstractValidator<DateAnswer>
{
    public DateAnswerValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
    }
}
