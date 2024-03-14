using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.UnitTests.Specifications;

public sealed class TestSpecification : Specification<TestEntity>
{
    public TestSpecification(BaseSortPaginatedRequest request)
    {
        Query.ApplyDefaultOrdering(request);
    }
}
