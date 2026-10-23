namespace Feature.Review.Rules.Update;

public class Validator : ReviewerRuleRequestValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.RuleId).NotEmpty();
    }
}
