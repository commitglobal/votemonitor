namespace Vote.Monitor.Answer.Module.UnitTests.Validators;

public class SelectedOptionRequestValidator : Validator<SelectedOptionRequest>
{
    public SelectedOptionRequestValidator()
    {
        RuleFor(x => x.OptionId).NotEmpty();
        RuleFor(x => x.Text).MaximumLength(1024);
    }
}
