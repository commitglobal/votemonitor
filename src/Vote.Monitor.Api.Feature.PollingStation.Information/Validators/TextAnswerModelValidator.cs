using Vote.Monitor.Api.Feature.PollingStation.Information.Models;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Validators;

public class TextAnswerModelValidator : Validator<TextAnswerModel>
{
    public TextAnswerModelValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(1024);
    }
}
