namespace Feature.NgoCoalitions.GuidesAccess;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.CoalitionId).NotEmpty();
        RuleFor(x => x.GuideId).NotEmpty();
        RuleForEach(x => x.NgoMembersIds).NotEmpty();
    }
}
