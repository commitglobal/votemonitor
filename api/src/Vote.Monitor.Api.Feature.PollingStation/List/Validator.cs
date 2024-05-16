namespace Vote.Monitor.Api.Feature.PollingStation.List;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.PageNumber).GreaterThan(0);
    }
}
