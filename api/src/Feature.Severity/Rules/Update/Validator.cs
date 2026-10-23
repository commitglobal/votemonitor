namespace Feature.Severity.Rules.Update;

public class Validator : SeverityRuleRequestValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.RuleId).NotEmpty();
    }
}
