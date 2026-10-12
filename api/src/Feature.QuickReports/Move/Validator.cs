namespace Feature.QuickReports.Move;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PollingStationId).NotEmpty();
    }
}
