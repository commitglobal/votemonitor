namespace Vote.Monitor.Api.Feature.Monitoring.AddObserver;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId)
            .NotEmpty();

        RuleFor(x => x.ObserverId)
            .NotEmpty();

        RuleFor(x => x.NgoId)
            .NotEmpty();
    }
}
