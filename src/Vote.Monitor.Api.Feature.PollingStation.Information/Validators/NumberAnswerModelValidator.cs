using Vote.Monitor.Api.Feature.PollingStation.Information.Models;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Validators;

public class NumberAnswerModelValidator : Validator<NumberAnswerModel>
{
    public NumberAnswerModelValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Value).GreaterThanOrEqualTo(0);
    }
}
