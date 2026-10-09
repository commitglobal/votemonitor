using Vote.Monitor.Domain.Entities.NgoAdminAggregate;
using Vote.Monitor.Domain.Entities.NgoStaffAggregate;

namespace Vote.Monitor.Domain.Entities.NgoAggregate;

public class Ngo : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public NgoStatus Status { get; private set; }
    public HashSet<NgoAdmin> Admins { get; private set; } = new();
    public HashSet<NgoStaff> Staff { get; private set; } = new();

    public Ngo(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        Status = NgoStatus.Activated;
    }

    public virtual void UpdateDetails(string name)
    {
        Name = name;
    }

    public virtual void Activate()
    {
        Status = NgoStatus.Activated;
    }

    public virtual void Deactivate()
    {
        Status = NgoStatus.Deactivated;
    }
    
#pragma warning disable CS8618 // Required by Entity Framework
    internal Ngo()
    {

    }
#pragma warning restore CS8618

}
