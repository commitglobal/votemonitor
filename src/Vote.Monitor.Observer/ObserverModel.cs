namespace Vote.Monitor.Observer;

public record ObserverModel
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Login { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, string>))]
    public required UserStatus Status { get; init; }
}
