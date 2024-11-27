namespace Feature.NgoCoalitions.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId)
            .NotEmpty();

        RuleFor(x => x.CoalitionId)
            .NotEmpty();

        RuleFor(x => x.CoalitionName)
            .NotEmpty()
            .MaximumLength(256);

        RuleForEach(x => x.NgoMembersIds).NotEmpty();
    }
}
