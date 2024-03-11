namespace Vote.Monitor.Domain.UnitTests.Specifications;

public class SpecificationExtensionsTests
{
    private readonly TestEntity _entity1 = new AuditableBaseEntityFaker(index: 1).Generate();
    private readonly TestEntity _entity2 = new AuditableBaseEntityFaker(index: 2).Generate();
    private readonly TestEntity _entity3 = new AuditableBaseEntityFaker(index: 3).Generate();
    private readonly TestEntity _entity4 = new AuditableBaseEntityFaker(index: 4).Generate();
    private readonly TestEntity _entity5 = new AuditableBaseEntityFaker(index: 5).Generate();

    private readonly TestEntity[] _testCollection;

    public SpecificationExtensionsTests()
    {
        _testCollection =
        [
            _entity5,
            _entity2,
            _entity1,
            _entity4,
            _entity3
        ];
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void ApplyDefaultOrdering_AppliesDefaultSorting_WhenNoSortColumnSet(string columnName)
    {
        // Arrange
        var request = new BaseSortPaginatedRequest
        {
            SortColumnName = columnName
        };

        var spec = new TestSpecification(request);

        // Act
        var result = spec.Evaluate(_testCollection).ToList();

        // Assert
        result.Should().BeInAscendingOrder(x => x.CreatedOn);
    }

    [Theory]
    [MemberData(nameof(CreatedOnSortTestData))]
    public void ApplyDefaultOrdering_SortsByCreatedOn(string columnName, SortOrder? sortOrder)
    {
        // Arrange
        var request = new BaseSortPaginatedRequest
        {
            SortColumnName = columnName,
            SortOrder = sortOrder
        };

        var spec = new TestSpecification(request);

        // Act
        var result = spec.Evaluate(_testCollection).ToList();

        // Assert
        result.Should().BeInAscendingOrder(x => x.CreatedOn);
    }

    [Theory]
    [MemberData(nameof(LastModifiedOnTestData))]
    public void ApplyDefaultOrdering_SortsByLastModifiedOn(string columnName, SortOrder? sortOrder)
    {
        // Arrange
        var request = new BaseSortPaginatedRequest
        {
            SortColumnName = columnName,
            SortOrder = sortOrder
        };

        var spec = new TestSpecification(request);

        // Act
        var result = spec.Evaluate(_testCollection).ToList();

        // Assert
        result.Should().BeInAscendingOrder(x => x.LastModifiedOn);
    }

    public static IEnumerable<object[]> CreatedOnSortTestData =>
        new List<object[]>
        {
            new object[] { "CreatedOn", SortOrder.Asc},
            new object[] { "createdOn", SortOrder.Asc},

            new object[] { "CreatedOn", null},
            new object[] { "createdOn", null}
        };

    public static IEnumerable<object[]> LastModifiedOnTestData =>
        new List<object[]>
        {
            new object[] { "LastModifiedOn", SortOrder.Asc },
            new object[] { "lastModifiedOn", SortOrder.Asc },

            new object[] { "LastModifiedOn", null },
            new object[] { "lastModifiedOn", null }
        };
}
