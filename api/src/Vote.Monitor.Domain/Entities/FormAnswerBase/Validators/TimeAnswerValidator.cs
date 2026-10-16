using FluentValidation;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;

public class TimeAnswerValidator : AbstractValidator<TimeAnswer>
{
    public TimeAnswerValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
    }
}
