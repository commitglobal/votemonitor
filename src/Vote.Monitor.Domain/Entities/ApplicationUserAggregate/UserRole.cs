using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public sealed class UserRole : SmartEnum<UserRole>
{
    public static readonly UserRole PlatformAdmin = new(nameof(PlatformAdmin), 1);
    public static readonly UserRole CSOAdmin = new(nameof(CSOAdmin), 2);
    public static readonly UserRole Observer = new(nameof(Observer), 3);

    private UserRole(string name, int value) : base(name, value)
    {
    }
}
