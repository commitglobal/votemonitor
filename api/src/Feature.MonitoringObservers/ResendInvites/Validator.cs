namespace Feature.MonitoringObservers.ResendInvites;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId)
            .NotEmpty();

        RuleFor(x => x.NgoId)
            .NotEmpty();

        RuleForEach(x => x.Ids)
            .NotEmpty();
    }
}
