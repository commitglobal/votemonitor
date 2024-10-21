namespace Vote.Monitor.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; internal set; }
    public DateTime CreatedOn { get; internal set; }

    protected BaseEntity(Guid id)
    {
        Id = id;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    protected BaseEntity()
    {
    }
#pragma warning restore CS8618 // Required by Entity Framework

}
