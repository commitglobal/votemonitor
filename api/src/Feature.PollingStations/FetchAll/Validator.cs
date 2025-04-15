namespace Feature.PollingStations.FetchAll;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
    }
}
