namespace Vote.Monitor.Api.Feature.PollingStation.FetchAllV2;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
    }
}
