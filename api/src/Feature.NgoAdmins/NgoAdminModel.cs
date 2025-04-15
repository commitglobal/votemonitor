namespace Feature.NgoAdmins;

public record NgoAdminModel
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    public required string? PhoneNumber { get; init; }
    
    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, string>))]
    public required UserStatus Status { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required DateTime? LastModifiedOn { get; init; }
}
