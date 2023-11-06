namespace Vote.Monitor.Api.Feature.CSO.Deactivate;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
