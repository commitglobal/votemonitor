namespace Vote.Monitor.Api.Feature.Observer.Services;
public class ObserverImportModel : IDuplicateCheck
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string Password { get => "string"; }
    public required string PhoneNumber { get; set; }

    public string DuplicateCheckValue { get => Email; }

}



