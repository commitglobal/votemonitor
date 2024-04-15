namespace Vote.Monitor.Api.Feature.Observer.Get;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
