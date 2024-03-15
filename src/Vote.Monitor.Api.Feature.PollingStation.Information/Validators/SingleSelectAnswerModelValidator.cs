using Vote.Monitor.Api.Feature.PollingStation.Information.Models;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Validators;

public class SingleSelectAnswerModelValidator : Validator<SingleSelectAnswerModel>
{
    public SingleSelectAnswerModelValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Selection).SetValidator(new SelectedOptionModelValidator());
    }
}
