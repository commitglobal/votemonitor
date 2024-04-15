namespace Vote.Monitor.Api.Feature.PollingStation.FetchLevels;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
    }
}
