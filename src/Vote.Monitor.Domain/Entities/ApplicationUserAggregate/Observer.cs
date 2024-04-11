using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public class Observer : ApplicationUser
{
    public virtual List<MonitoringObserver> MonitoringObservers { get; internal set; } = [];

    public string PhoneNumber { get; private set; }
    private Observer(string name,
        string login,
        string password,
        string phoneNumber) : base(name, login, password, UserRole.Observer)
    {
        PhoneNumber = phoneNumber;
    }

    public static Observer Create(string name, string email, string password, string phone)
    {
        return new Observer(name, email, password, phone);
    }



#pragma warning disable CS8618 // Required by Entity Framework
    private Observer()
    {

    }
#pragma warning restore CS8618

}
