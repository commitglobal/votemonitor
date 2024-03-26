namespace Vote.Monitor.Domain.UnitTests.Specifications;

public class TestEntity : AuditableBaseEntity
{
    public TestEntity(ITimeProvider timeProvider) : base(Guid.NewGuid())
    {
    }
}
