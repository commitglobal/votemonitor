namespace Vote.Monitor.Api.Feature.Monitoring.AddNgo;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.NgoId)
            .NotEmpty();
    }
}
