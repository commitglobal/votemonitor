using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Constants;

namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public abstract class ApplicationUser : AuditableBaseEntity, IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    protected ApplicationUser()
    {

    }
#pragma warning restore CS8618

    public string Name { get; private set; }
    public string Login { get; private set; }
    public string Password { get; private set; }
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }
    public UserPreferences Preferences { get; private set; }

    public ApplicationUser(string name,
        string login,
        string password,
        UserRole role) : base(Guid.NewGuid())
    {
        Name = name;
        Login = login;
        Password = password;
        Role = role;
        Status = UserStatus.Active;
        Preferences = UserPreferences.Defaults;
    }

    public void UpdateDetails(string name)
    {
        Name = name;
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

public class UserPreferences
{
#pragma warning disable CS8618 // Required by Entity Framework
    protected UserPreferences()
    {
    }
#pragma warning restore CS8618

    protected UserPreferences(Guid languageId)
    {
        LanguageId = languageId;
    }

    public static UserPreferences Defaults => new(LanguagesList.EN.Id);
    public Guid LanguageId { get; private set; }

    public void Update(Guid languageId)
    {
        LanguageId = languageId;
    }
}
