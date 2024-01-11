namespace Vote.Monitor.Core.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; protected init; }
}
