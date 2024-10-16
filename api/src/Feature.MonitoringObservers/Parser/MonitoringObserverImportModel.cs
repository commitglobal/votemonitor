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

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        MonitoringObserverImportModel other = (MonitoringObserverImportModel)obj;
        return Email == other.Email;
    }
}