using Vote.Monitor.Domain.Entities.CSOAggregate;

namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public class CSOAdmin : ApplicationUser
{

#pragma warning disable CS8618 // Required by Entity Framework
    private CSOAdmin()
    {

    }

    public Guid CSOId { get; private set; }
    public CSO CSO{ get; set; }

    public CSOAdmin(Guid csoId, string name, string login, string password, UserRole role) : base(name, login, password, role)
    {
        CSOId = csoId;
    }
}
