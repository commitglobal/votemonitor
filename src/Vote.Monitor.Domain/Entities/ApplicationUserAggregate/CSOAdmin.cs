using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.CSOAggregate;

namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public class CSOAdmin : ApplicationUser
{

#pragma warning disable CS8618 // Required by Entity Framework
    private CSOAdmin()
    {

    }
#pragma warning restore CS8618

    public Guid CSOId { get; private set; }
    public CSO CSO{ get; set; }

    public CSOAdmin(Guid csoId, string name, string login, string password, ITimeService timeService) : base(name, login, password, UserRole.CSOAdmin, timeService)
    {
        CSOId = csoId;
    }
}
