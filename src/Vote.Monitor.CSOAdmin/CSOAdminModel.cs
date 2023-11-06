namespace Vote.Monitor.CSOAdmin;

public record CSOAdminModel
{
    public required Guid Id { get; init; }
    public required string Login { get; init; }
    public required string Name { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, string>))]
    public required UserStatus Status { get; init; }
}
