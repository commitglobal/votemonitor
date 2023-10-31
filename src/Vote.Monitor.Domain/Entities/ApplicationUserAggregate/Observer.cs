namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public class Observer : ApplicationUser
{
#pragma warning disable CS8618 // Required by Entity Framework
    private Observer()
    {

    }

    public Observer(string name, string login, string password) : base(name, login, password, UserRole.Observer)
    {
    }
}
