namespace Feature.PollingStation.Visits;

public record VisitModel
{
    public Guid PollingStationId { get; set; }
    public DateTime VisitedAt { get; set; }

    public string Level1 { get; set; }
    public string Level2 { get; set; }
    public string Level3 { get; set; }
    public string Level4 { get; set; }
    public string Level5 { get; set; }
    public string Address { get; set; }
    public string Number { get; set; }
}
