namespace Vote.Monitor.Api.Feature.Observer;

public record ObserverModel
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Login { get; init; }

    public required string PhoneNumber { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, string>))]
    public required UserStatus Status { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required DateTime? LastModifiedOn { get; init; }
}
