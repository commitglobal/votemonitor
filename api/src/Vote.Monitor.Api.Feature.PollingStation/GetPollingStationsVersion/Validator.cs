namespace Vote.Monitor.Api.Feature.PollingStation.GetPollingStationsVersion;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
    }
}
