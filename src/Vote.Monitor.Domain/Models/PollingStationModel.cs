namespace Vote.Monitor.Domain.Models;
public class PollingStationModel
{
    public Guid Id { get; set; }
    public required string Address { get; set; }
    public int DisplayOrder { get; set; }
    public List<TagModel> Tags { get; set; } = new List<TagModel>();
}
