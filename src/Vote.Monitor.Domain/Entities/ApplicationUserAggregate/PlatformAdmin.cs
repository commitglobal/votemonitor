namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public class PlatformAdmin : ApplicationUser
{
#pragma warning disable CS8618
    private PlatformAdmin()
    {
    }

    public PlatformAdmin(string name, string login, string password, UserRole role) : base(name, login, password, role)
    {
    }
}
