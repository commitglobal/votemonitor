using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedOn { get; private set; }

    protected BaseEntity(Guid id, ITimeService timeService)
    {
        Id = id;
        CreatedOn = timeService.UtcNow;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    protected BaseEntity()
    {
    }
#pragma warning restore CS8618 // Required by Entity Framework

}
