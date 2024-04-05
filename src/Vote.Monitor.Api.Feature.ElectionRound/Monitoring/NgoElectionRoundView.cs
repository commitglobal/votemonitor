namespace Vote.Monitor.Api.Feature.ElectionRound.Monitoring;

public class NgoElectionRoundView
{
    public Guid MonitoringNgoId { get; set; }
    public string Title { get; set; }
    public string EnglishTitle { get; set; }
    public DateOnly StartDate { get; set; }
    public string Country { get; set; }
    public Guid CountryId { get; set; }
}
