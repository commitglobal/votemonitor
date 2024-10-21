namespace Vote.Monitor.Domain.Entities;

public abstract class AuditableBaseEntity
{
    public DateTime CreatedOn { get; internal set; }
    public Guid CreatedBy { get; internal set; }
    public DateTime? LastModifiedOn { get; internal set; }
    public Guid LastModifiedBy { get; internal set; }

#pragma warning disable CS8618 // Required by Entity Framework
    protected AuditableBaseEntity() : base()
    {
    }
#pragma warning restore CS8618 // Required by Entity Framework

}
