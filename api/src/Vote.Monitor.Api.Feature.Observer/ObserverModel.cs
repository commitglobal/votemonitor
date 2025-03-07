namespace Vote.Monitor.Api.Feature.Observer;

public record ObserverModel
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }

    public string? PhoneNumber { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, string>))]
    public UserStatus Status { get; init; }

    public DateTime CreatedOn { get; init; }
    public DateTime? LastModifiedOn { get; init; }
}
