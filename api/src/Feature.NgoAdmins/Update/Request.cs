namespace Feature.NgoAdmins.Update;

public class Request
{
    public Guid NgoId { get; set; }
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? PhoneNumber { get; set; }
}
