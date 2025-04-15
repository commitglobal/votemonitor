using FastEndpoints;
using FluentValidation;
using Module.Answers.Requests;

namespace Module.Answers.Validators;

public class TextAnswerRequestValidator : Validator<TextAnswerRequest>
{
    public TextAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(1024);
    }
}
