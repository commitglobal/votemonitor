namespace Vote.Monitor.Api.Feature.ElectionRound.Monitoring;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.NgoId).NotEmpty();
    }
}
