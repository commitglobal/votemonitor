using Microsoft.AspNetCore.Identity;

namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public class ApplicationUser : IdentityUser<Guid>, IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    protected ApplicationUser()
    {

    }
#pragma warning restore CS8618

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime RefreshTokenExpiryTime { get; private set; }
    public UserStatus Status { get; private set; }
    public UserPreferences Preferences { get; private set; }

    private ApplicationUser(string firstName, string lastName, string email, string password, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        UserName = email;
        NormalizedEmail = email.ToUpperInvariant();
        NormalizedUserName = email.ToUpperInvariant();
        PhoneNumber = phoneNumber;

        Status = UserStatus.Active;
        Preferences = UserPreferences.Defaults;

        var hasher = new PasswordHasher<ApplicationUser>();
        PasswordHash = hasher.HashPassword(this, password);
    }

    private ApplicationUser(string firstName, string lastName, string email, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        UserName = email;
        NormalizedEmail = email.ToUpperInvariant();
        NormalizedUserName = email.ToUpperInvariant();
        PhoneNumber = phoneNumber;
        Status = UserStatus.Active;
        Preferences = UserPreferences.Defaults;
    }

    public static ApplicationUser CreateForInvite(string firstName, string lastName, string email, string phoneNumber) => 
        new(firstName, lastName, email, phoneNumber);

    public static ApplicationUser Create(string firstName, string lastName, string email, string phoneNumber, string password) =>
         new(firstName, lastName, email, password, phoneNumber);

    public void UpdateDetails(string firstName, string lastName, string phoneNumber)
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
