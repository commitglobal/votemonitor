using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.UnitTests.Specifications;

public class TestSpecification : Specification<TestEntity>
{
    public TestSpecification(BaseFilterRequest filter)
    {
        Query.ApplyDefaultOrdering(filter);
    }
}
