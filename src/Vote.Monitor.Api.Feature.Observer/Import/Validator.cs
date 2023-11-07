namespace Vote.Monitor.Api.Feature.Observer.Import;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.CSOId)
            .NotEmpty();
    }
}
