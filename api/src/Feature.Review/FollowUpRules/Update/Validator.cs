namespace Feature.Review.FollowUpRules.Update;

public class Validator : FollowUpRuleRequestValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.RuleId).NotEmpty();
    }
}
