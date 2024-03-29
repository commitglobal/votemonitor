namespace Vote.Monitor.Api.Feature.PollingStation.FetchAllV3;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
    }
}
