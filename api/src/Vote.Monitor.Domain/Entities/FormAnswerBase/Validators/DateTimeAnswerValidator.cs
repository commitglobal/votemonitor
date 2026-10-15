using FluentValidation;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;

public class DateTimeAnswerValidator : AbstractValidator<DateTimeAnswer>
{
    public DateTimeAnswerValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.DateTime).NotEmpty();
    }
}
