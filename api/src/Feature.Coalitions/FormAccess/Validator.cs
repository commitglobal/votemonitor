namespace Feature.NgoCoalitions.FormAccess;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.CoalitionId).NotEmpty();
        RuleFor(x => x.FormId).NotEmpty();
        RuleForEach(x => x.NgoMembersIds).NotEmpty();
    }
}
