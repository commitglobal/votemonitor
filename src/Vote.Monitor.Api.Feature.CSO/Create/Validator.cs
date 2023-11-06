namespace Vote.Monitor.Api.Feature.CSO.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .NotEmpty();
    }
}
