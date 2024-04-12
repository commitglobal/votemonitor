namespace Feature.MonitoringObservers.Tag;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId)
            .NotEmpty();

        RuleFor(x => x.MonitoringNgoId)
            .NotEmpty();

        RuleFor(x => x.Tags)
            .NotEmpty();

        RuleFor(x => x.MonitoringObserverIds)
            .NotEmpty();

        RuleForEach(x => x.Tags)
            .NotEmpty();

        RuleForEach(x => x.MonitoringObserverIds)
            .NotEmpty();
    }
}
