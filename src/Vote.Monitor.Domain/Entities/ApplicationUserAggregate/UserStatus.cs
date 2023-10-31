using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public sealed class UserStatus : SmartEnum<UserStatus>
{
    public static readonly UserStatus Active = new(nameof(Active), 1);
    public static readonly UserStatus Deactivated = new(nameof(Deactivated), 2);

    private UserStatus(string name, int value) : base(name, value)
    {
    }
}
