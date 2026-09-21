namespace Feature.Statistics.GetMonitoringObserverStats;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.MonitoringObserverId).NotEmpty();
        RuleFor(x => x.DataSource).NotEmpty();
    }
}
