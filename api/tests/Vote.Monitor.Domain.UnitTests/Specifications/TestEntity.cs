namespace Vote.Monitor.Domain.UnitTests.Specifications;

public class TestEntity : AuditableBaseEntity
{
    public TestEntity() : base(Guid.NewGuid())
    {
    }
}
