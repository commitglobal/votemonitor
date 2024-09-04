namespace Vote.Monitor.Api.Feature.Monitoring.EnableCitizenReportingNgo;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid NgoId { get; set; }
}
