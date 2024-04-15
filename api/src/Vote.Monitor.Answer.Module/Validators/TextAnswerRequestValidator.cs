using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Answer.Module.Requests;

namespace Vote.Monitor.Answer.Module.Validators;

public class TextAnswerRequestValidator : Validator<TextAnswerRequest>
{
    public TextAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(1024);
    }
}
