using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Domain.Entities.NgoStaffAggregate;

public class NgoStaff : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ApplicationUserId { get; private set; }
    public ApplicationUser ApplicationUser { get; private set; }
    public Guid NgoId { get; private set; }
    public Ngo Ngo { get; private set; }

    public NgoStaff(Guid ngoId, ApplicationUser applicationUser)
    {
        Id = applicationUser.Id;
        NgoId = ngoId;
        ApplicationUser = applicationUser;
        ApplicationUserId = applicationUser.Id;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private NgoStaff()
    {
    }
#pragma warning restore CS8618
}
