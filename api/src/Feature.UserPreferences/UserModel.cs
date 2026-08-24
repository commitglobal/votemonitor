using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Feature.UserPreferences;

public record UserModel
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string DisplayName { get; init; }
    public required string? PhoneNumber { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<UserRole, string>))]
    public required UserRole Role { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<UserStatus, string>))]
    public required UserStatus Status { get; init; }

    public required UserPreferencesModel Preferences { get; init; }
}
