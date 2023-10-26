using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Vote.Monitor.Domain.Entities.CSOAggregate;

public class CSO : BaseEntity, IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    private CSO()
    {

    }

    public string Name { get; private set; }
    public CSOStatus Status { get; private set; }
    public HashSet<CSOAdmin> Admins { get; set; } = new();

    public CSO(string name)
    {
        Name = name;
        Status = CSOStatus.Active;
    }

    public void UpdateDetails(string name)
    {
        Name = name;
    }

    public void MarkAsActive()
    {
        // TODO: handle invariants
        Status = CSOStatus.Active;
    }

    public void MarkAsInactive()
    {
        // TODO: handle invariants
        Status = CSOStatus.Inactive;
    }
}
