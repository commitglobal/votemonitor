namespace Vote.Monitor.Api.Feature.Monitoring.GetMonitoringNgos;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
    }
}
