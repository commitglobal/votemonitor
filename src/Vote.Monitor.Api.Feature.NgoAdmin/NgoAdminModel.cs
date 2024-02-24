namespace Vote.Monitor.Api.Feature.NgoAdmin;

public record NgoAdminModel
{
    public required Guid Id { get; init; }
    public required string Login { get; init; }
    public required string Name { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, string>))]
    public required UserStatus Status { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required DateTime? LastModifiedOn { get; init; }
}
