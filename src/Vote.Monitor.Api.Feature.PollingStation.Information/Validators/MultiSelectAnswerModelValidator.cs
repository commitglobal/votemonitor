using Vote.Monitor.Api.Feature.PollingStation.Information.Models;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Validators;

public class MultiSelectAnswerModelValidator : Validator<MultiSelectAnswerModel>
{
    public MultiSelectAnswerModelValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Selection).NotEmpty();
        RuleForEach(x => x.Selection).SetValidator(new SelectedOptionModelValidator());
    }
}
