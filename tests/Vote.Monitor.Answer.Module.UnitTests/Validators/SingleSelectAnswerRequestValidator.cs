namespace Vote.Monitor.Answer.Module.UnitTests.Validators;

public class SingleSelectAnswerRequestValidator : Validator<SingleSelectAnswerRequest>
{
    public SingleSelectAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.Selection).SetValidator(new SelectedOptionRequestValidator());
    }
}
