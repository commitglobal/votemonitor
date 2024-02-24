using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public sealed class UserRole : SmartEnum<UserRole, string>
{
    public static readonly UserRole PlatformAdmin = new(nameof(PlatformAdmin), nameof(PlatformAdmin));
    public static readonly UserRole NgoAdmin = new(nameof(NgoAdmin), nameof(NgoAdmin));
    public static readonly UserRole Observer = new(nameof(Observer), nameof(Observer));

    private UserRole(string name, string value) : base(name, value)
    {
    }
}
