namespace SubmissionsFaker.Clients.PollingStations;

public class PollingStationNode
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Depth { get; set; }
    public int? ParentId { get; set; }
    public string? Number { get; set; }
    public Guid? PollingStationId { get; set; }
    public int? DisplayOrder { get; set; }
}
