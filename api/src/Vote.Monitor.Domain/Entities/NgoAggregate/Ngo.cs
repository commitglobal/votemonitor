using Vote.Monitor.Domain.Entities.NgoAdminAggregate;

namespace Vote.Monitor.Domain.Entities.NgoAggregate;

public class Ngo : AuditableBaseEntity, IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    internal Ngo()
    {

    }
#pragma warning restore CS8618

    public string Name { get; private set; }
    public NgoStatus Status { get; private set; }
    public HashSet<NgoAdmin> Admins { get; private set; } = new();

    public Ngo(string name) : base(Guid.NewGuid())
    {
        Name = name;
        Status = NgoStatus.Activated;
    }

    public virtual void UpdateDetails(string name)
    {
        Name = name;
    }

    public virtual void Activate()
    {
        // TODO: handle invariants
        Status = NgoStatus.Activated;
    }

    public virtual void Deactivate()
    {
        // TODO: handle invariants
        Status = NgoStatus.Deactivated;
    }
}
