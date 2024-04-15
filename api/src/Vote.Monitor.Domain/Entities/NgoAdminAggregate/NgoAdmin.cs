using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Domain.Entities.NgoAdminAggregate;

public class NgoAdmin : AuditableBaseEntity, IAggregateRoot
{
    public Guid ApplicationUserId { get; private set; }
    public ApplicationUser ApplicationUser { get; private set; }
    public Guid NgoId { get; private set; }
    public Ngo Ngo { get; private set; }

    public NgoAdmin(Guid ngoId, ApplicationUser applicationUser) : base(applicationUser.Id)
    {
        NgoId = ngoId;
        ApplicationUser = applicationUser;
        ApplicationUserId = applicationUser.Id;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private NgoAdmin()
    {
    }
#pragma warning restore CS8618
}
