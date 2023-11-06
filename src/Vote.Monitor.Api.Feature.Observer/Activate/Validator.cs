namespace Vote.Monitor.Api.Feature.Observer.Activate;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
