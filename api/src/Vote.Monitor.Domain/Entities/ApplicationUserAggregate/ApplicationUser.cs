using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public class ApplicationUser : IdentityUser<Guid>, IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    protected ApplicationUser()
    {
    }
#pragma warning restore CS8618

    public UserRole Role { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string DisplayName { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime RefreshTokenExpiryTime { get; private set; }
    public UserStatus Status { get; private set; }
    public UserPreferences Preferences { get; private set; }
    public string? InvitationToken { get; private set; } = null;

    private ApplicationUser(UserRole role, string firstName, string lastName, string email, string? phoneNumber,
        string password)
    {
        Role = role;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim();
        UserName = email.Trim();
        NormalizedEmail = email.Trim().ToUpperInvariant();
        NormalizedUserName = email.Trim().ToUpperInvariant();
        PhoneNumber = phoneNumber?.Trim();

        Status = UserStatus.Pending;
        Preferences = UserPreferences.Defaults;

        if (string.IsNullOrEmpty(password.Trim()))
        {
            NewInvite();
        }
        else
        {
            var hasher = new PasswordHasher<ApplicationUser>();
            PasswordHash = hasher.HashPassword(this, password.Trim());
        }
    }

    public static ApplicationUser Invite(string firstName, string lastName, string email, string? phoneNumber) =>
        new(UserRole.Observer, firstName, lastName, email, phoneNumber, string.Empty);

    public static ApplicationUser CreatePlatformAdmin(string firstName, string lastName, string email, string password) =>
        new(UserRole.PlatformAdmin, firstName, lastName, email, null, password);

    public static ApplicationUser CreateNgoAdmin(string firstName, string lastName, string email, string? phoneNumber,
        string password) =>
        new(UserRole.NgoAdmin, firstName, lastName, email, phoneNumber, password);

    public static ApplicationUser CreateObserver(string firstName, string lastName, string email, string? phoneNumber,
        string password) =>
        new(UserRole.Observer, firstName, lastName, email, phoneNumber, password);

    public void UpdateDetails(string firstName, string lastName, string? phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
    }

    public void UpdateRefreshToken(string refreshToken, DateTime refreshTokenExpiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = refreshTokenExpiryTime;
    }

    public void AcceptInvite(string password)
    {
        InvitationToken = null;
        var hasher = new PasswordHasher<ApplicationUser>();
        PasswordHash = hasher.HashPassword(this, password);
        EmailConfirmed = true;
        Status = UserStatus.Active;
    }

    public void NewInvite()
    {
        InvitationToken = Guid.NewGuid().ToString("N");
    }

    public void Activate()
    {
        // TODO: handle invariants
        Status = UserStatus.Active;
    }

    public void Deactivate()
    {
        // TODO: handle invariants
        Status = UserStatus.Deactivated;
    }
}
