using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public class NgoAdmin : ApplicationUser
{

#pragma warning disable CS8618 // Required by Entity Framework
    private NgoAdmin()
    {

    }
#pragma warning restore CS8618

    public Guid NgoId { get; private set; }
    public Ngo Ngo{ get; private set; }

    public NgoAdmin(Guid ngoId, string name, string login, string password, ITimeProvider timeProvider) : base(name, login, password, UserRole.NgoAdmin, timeProvider)
    {
        NgoId = ngoId;
    }
}
