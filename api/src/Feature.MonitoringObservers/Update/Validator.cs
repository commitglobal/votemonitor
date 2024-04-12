namespace Feature.MonitoringObservers.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId)
            .NotEmpty();

        RuleFor(x => x.MonitoringNgoId)
            .NotEmpty();

        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Status)
            .NotNull()
            .NotEmpty();

        RuleForEach(x => x.Tags)
            .NotEmpty();
    }
}
