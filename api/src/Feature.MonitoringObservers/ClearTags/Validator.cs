namespace Feature.MonitoringObservers.ClearTags;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId)
            .NotEmpty();

        RuleFor(x => x.NgoId)
            .NotEmpty();

        RuleFor(x => x.MonitoringObserverIds)
            .NotEmpty();

        RuleForEach(x => x.MonitoringObserverIds)
            .NotEmpty();
    }
}
