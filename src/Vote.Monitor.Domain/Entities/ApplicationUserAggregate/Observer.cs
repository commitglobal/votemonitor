using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public class Observer : ApplicationUser
{
#pragma warning disable CS8618 // Required by Entity Framework
    private Observer()
    {

    }
#pragma warning restore CS8618

    public string PhoneNumber { get; private set; }
    public Observer(string name,
        string login,
        string password,
        string phoneNumber) : base(name, login, password, UserRole.Observer)
    {
        PhoneNumber = phoneNumber;
    }
}
