namespace Vote.Monitor.Domain.Entities.CSOAggregate;

public class CSO : AuditableBaseEntity, IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    internal CSO()
    {

    }
#pragma warning restore CS8618

    public string Name { get; private set; }
    public CSOStatus Status { get; private set; }
    public HashSet<CSOAdmin> Admins { get; private set; } = new();

    public CSO(string name, ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        Name = name;
        Status = CSOStatus.Activated;
    }

    public virtual void UpdateDetails(string name)
    {
        Name = name;
    }

    public virtual void Activate()
    {
        // TODO: handle invariants
        Status = CSOStatus.Activated;
    }

    public virtual void Deactivate()
    {
        // TODO: handle invariants
        Status = CSOStatus.Deactivated;
    }
}
