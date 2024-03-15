using Vote.Monitor.Api.Feature.PollingStation.Information.Models;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Validators;

public class DateAnswerModelValidator : Validator<DateAnswerModel>
{
    public DateAnswerModelValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
    }
}
