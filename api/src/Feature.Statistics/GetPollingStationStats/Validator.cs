namespace Feature.Statistics.GetPollingStationStats;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.PollingStationId).NotEmpty();
        RuleFor(x => x.DataSource).NotEmpty();
    }
}
