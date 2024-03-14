using Vote.Monitor.Api.Feature.PollingStation.Information.Models;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Validators;

public class SelectedOptionModelValidator : Validator<SelectedOptionModel>
{
    public SelectedOptionModelValidator()
    {
        RuleFor(x => x.OptionId).NotEmpty();
        RuleFor(x => x.Text).MaximumLength(1024);
    }
}
