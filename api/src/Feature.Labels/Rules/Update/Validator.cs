namespace Feature.Labels.Rules.Update;

public class Validator : LabelRuleRequestValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.RuleId).NotEmpty();
    }
}
