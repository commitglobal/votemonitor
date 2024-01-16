namespace Vote.Monitor.Domain.UnitTests.Specifications;

public class TestEntity : AuditableBaseEntity
{
    public TestEntity(ITimeService timeService) : base(Guid.NewGuid(), timeService)
    {
    }
}
