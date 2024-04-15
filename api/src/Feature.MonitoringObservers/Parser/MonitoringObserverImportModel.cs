namespace Feature.MonitoringObservers.Parser;

public class MonitoringObserverImportModel
{
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string[] Tags { get; set; }

    public override int GetHashCode()
    {
        return Email.GetHashCode();
    }
}
