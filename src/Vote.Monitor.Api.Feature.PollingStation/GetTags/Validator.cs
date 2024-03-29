namespace Vote.Monitor.Api.Feature.PollingStation.GetTags;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
    }
}
