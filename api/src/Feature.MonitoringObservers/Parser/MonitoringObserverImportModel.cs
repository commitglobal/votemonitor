namespace Feature.MonitoringObservers.Parser;

public class MonitoringObserverImportModel
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string[] Tags { get; set; }

    public override int GetHashCode()
    {
        return Email.GetHashCode();
    }
}
