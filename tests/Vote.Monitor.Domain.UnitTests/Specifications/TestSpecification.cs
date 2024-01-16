namespace Vote.Monitor.Domain.UnitTests.Specifications;

public class TestSpecification : Specification<TestEntity>
{
    public TestSpecification(BaseSortPaginatedRequest request)
    {
        Query.ApplyDefaultOrdering(request);
    }
}
