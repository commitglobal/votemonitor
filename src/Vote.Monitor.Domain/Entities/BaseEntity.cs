namespace Vote.Monitor.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedOn { get; private set; }

    protected BaseEntity(Guid id, ITimeProvider timeProvider)
    {
        Id = id;
        CreatedOn = timeProvider.UtcNow;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    protected BaseEntity()
    {
    }
#pragma warning restore CS8618 // Required by Entity Framework

}
