namespace Vote.Monitor.Api.Feature.CSO.Activate;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
