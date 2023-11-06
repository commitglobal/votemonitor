using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public sealed class UserStatus : SmartEnum<UserStatus, string>
{
    public static readonly UserStatus Active = new(nameof(Active), nameof(Active));
    public static readonly UserStatus Deactivated = new(nameof(Deactivated), nameof(Deactivated));

    private UserStatus(string name, string value) : base(name, value)
    {
    }
}
