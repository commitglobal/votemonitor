namespace Vote.Monitor.Api.Feature.Observer.Services;

public class ObserverImportModel

{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string Password { get => "string"; }
    public required string PhoneNumber { get; set; }


    public override int GetHashCode()
    {
        return Email.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not ObserverImportModel) return false;
        if (ReferenceEquals(this, obj)) return true;

        ObserverImportModel observerImportModel = (ObserverImportModel)obj;

        return Email.Equals(observerImportModel.Email);
    }

}





