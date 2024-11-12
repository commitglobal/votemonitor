namespace Feature.NgoCoalitions.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.LeaderId).NotEmpty();
        RuleForEach(x => x.NgoMembersIds).NotEmpty();
        RuleFor(x => x.CoalitionName).NotEmpty().MaximumLength(256);
    }
}
