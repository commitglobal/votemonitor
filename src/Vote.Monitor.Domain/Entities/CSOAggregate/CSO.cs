using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Vote.Monitor.Domain.Entities.CSOAggregate;

public class CSO : AuditableBaseEntity, IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    private CSO()
    {

    }
#pragma warning restore CS8618

    public string Name { get; private set; }
    public CSOStatus Status { get; private set; }
    public HashSet<CSOAdmin> Admins { get; set; } = new();

    public CSO(string name, ITimeService timeService) : base(Guid.NewGuid(), timeService)
    {
        Name = name;
        Status = CSOStatus.Activated;
    }

    public void UpdateDetails(string name)
    {
        Name = name;
    }

    public void Activate()
    {
        // TODO: handle invariants
        Status = CSOStatus.Activated;
    }

    public void Deactivate()
    {
        // TODO: handle invariants
        Status = CSOStatus.Deactivated;
    }
}
