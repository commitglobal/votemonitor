namespace Vote.Monitor.Api.Feature.Observer.Parser;

public record ObserverImportModel
{
    public required string LastName { get; init; }
    public required string FirstName { get; init; }
    public required string Email { get; init; }
    public required string? PhoneNumber { get; init; }
    public required string Password { get; init; }

    public virtual bool Equals(ObserverImportModel? other)
    {
        return other?.Email == Email;
    }

    public override int GetHashCode()
    {
        return Email.GetHashCode();
    }
}
