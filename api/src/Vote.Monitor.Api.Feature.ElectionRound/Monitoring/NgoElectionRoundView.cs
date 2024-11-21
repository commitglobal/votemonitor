namespace Vote.Monitor.Api.Feature.ElectionRound.Monitoring;

public class NgoElectionRoundView
{
    public Guid MonitoringNgoId { get; set; }
    public Guid ElectionRoundId { get; set; }
    public string Title { get; set; }
    public string EnglishTitle { get; set; }
    public DateOnly StartDate { get; set; }
    public string Country { get; set; }
    public Guid CountryId { get; set; }
    public bool IsMonitoringNgoForCitizenReporting { get; set; }
    public bool IsCoalitionLeader { get; set; }
    public ElectionRoundStatus Status { get; set; }

    public Guid? CoalitionId { get; set; }
    public string? CoalitionName { get; set; }
}
