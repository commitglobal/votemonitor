namespace Vote.Monitor.Api.Feature.Country.Get;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
