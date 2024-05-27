namespace Vote.Monitor.Domain.Entities.Auditing;

public class Trail : BaseEntity
{
    public Trail() : base(Guid.NewGuid())
    {
    }

    public Guid UserId { get; set; }
    public string? Type { get; set; }
    public string? TableName { get; set; }
    public DateTime Timestamp { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public string? PrimaryKey { get; set; }
}
