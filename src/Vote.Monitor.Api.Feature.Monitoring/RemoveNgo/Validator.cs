namespace Vote.Monitor.Api.Feature.Monitoring.RemoveNgo;

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
