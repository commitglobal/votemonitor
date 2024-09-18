namespace SubmissionsFaker.Clients.Locations;

public class LocationNode
{
    public int Id { get; set; }
    public int Depth { get; set; }
    public int? ParentId { get; set; }
    public Guid? LocationId { get; set; }
    public int? DisplayOrder { get; set; }
}
