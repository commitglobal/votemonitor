namespace Vote.Monitor.Api.Feature.PollingStation.Information.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.PollingStationId).NotEmpty();
        RuleFor(x => x.ObserverId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
