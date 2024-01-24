namespace Vote.Monitor.Api.Feature.Monitoring.RemoveObserver;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.ObserverId)
            .NotEmpty();
    }
}
