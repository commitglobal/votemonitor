using FluentValidation;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;

public class NumberAnswerValidator : AbstractValidator<NumberAnswer>
{
    public NumberAnswerValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Value).GreaterThanOrEqualTo(0);
    }
}
