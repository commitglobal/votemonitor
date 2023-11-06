namespace Vote.Monitor.Api.Feature.CSOAdmin.Import;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.CSOId)
            .NotEmpty();
    }
}
