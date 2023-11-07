namespace Vote.Monitor.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; protected init; }
}
