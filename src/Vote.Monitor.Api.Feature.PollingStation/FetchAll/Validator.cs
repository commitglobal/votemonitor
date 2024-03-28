namespace Vote.Monitor.Api.Feature.PollingStation.FetchAll;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
    }
}
