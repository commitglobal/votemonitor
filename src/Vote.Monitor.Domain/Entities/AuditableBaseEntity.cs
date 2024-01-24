namespace Vote.Monitor.Domain.Entities;

public abstract class AuditableBaseEntity : BaseEntity
{
    public Guid CreatedBy { get; internal set; }
    public DateTime? LastModifiedOn { get; internal set; }
    public Guid LastModifiedBy { get; internal set; }
    protected AuditableBaseEntity(Guid id, ITimeProvider timeProvider) : base(id, timeProvider)
    {
        LastModifiedOn = timeProvider.UtcNow;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    protected AuditableBaseEntity() : base()
    {
    }
#pragma warning restore CS8618 // Required by Entity Framework

}
