using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.ObserverAggregate;

public class Observer : AuditableBaseEntity, IAggregateRoot
{
    public Guid ApplicationUserId { get; private set; }
    public ApplicationUser ApplicationUser { get; private set; }

    public virtual List<MonitoringObserver> MonitoringObservers { get; internal set; } = [];

    private Observer(ApplicationUser applicationUser) : base(applicationUser.Id)
    {
        ApplicationUser = applicationUser;
        ApplicationUserId = applicationUser.Id;
    }

    public static Observer Create(ApplicationUser applicationUser)
    {
        return new Observer(applicationUser);
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private Observer()
    {
    }
#pragma warning restore CS8618

}
