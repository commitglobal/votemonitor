using FluentValidation;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase.Validators;

public class TextAnswerValidator : AbstractValidator<TextAnswer>
{
    public TextAnswerValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Text).MaximumLength(1024);
    }
}
