using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public class PlatformAdmin : ApplicationUser
{
#pragma warning disable CS8618
    private PlatformAdmin()
    {
    }
#pragma warning restore CS8618

    public PlatformAdmin(string name,
        string login,
        string password) : base(name, login, password, UserRole.PlatformAdmin)
    {
    }
}
