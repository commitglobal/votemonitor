namespace Feature.Review.Models;

/// <summary>An NGO admin or staff member who can review records.</summary>
public record NgoMemberModel
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
    public required string Email { get; init; }
    public required UserRole Role { get; init; }
}
