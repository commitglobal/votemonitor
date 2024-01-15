using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.UnitTests.Specifications;

public class SpecificationExtensionsTests
{
    private readonly TestEntity _observer1 = new AuditableBaseEntityFaker(index: 1).Generate();
    private readonly TestEntity _observer2 = new AuditableBaseEntityFaker(index: 2).Generate();
    private readonly TestEntity _observer3 = new AuditableBaseEntityFaker(index: 3).Generate();
    private readonly TestEntity _observer4 = new AuditableBaseEntityFaker(index: 4).Generate();
    private readonly TestEntity _observer5 = new AuditableBaseEntityFaker(index: 5).Generate();

    private readonly TestEntity[] _testCollection;

    public SpecificationExtensionsTests()
    {
        _testCollection =
        [
            _observer5,
            _observer2,
            _observer1,
            _observer4,
            _observer3
        ];
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void ApplyDefaultOrdering_AppliesDefaultSorting_WhenNoSortColumnSet(string columnName)
    {
        // Arrange
        var request = new BaseFilterRequest
        {
            ColumnName = columnName,
        };

        var spec = new TestSpecification(request);

        // Act
        var result = spec.Evaluate(_testCollection).ToList();

        // Assert
        result
            .Should()
            .HaveCount(5)
            .And
            .BeInAscendingOrder(x => x.CreatedOn);
    }

    [Theory]
    [MemberData(nameof(CreatedOnSortTestData))]
    public void ApplyDefaultOrdering_SortsByCreatedOn(string columnName, SortOrder? sortOrder)
    {
        // Arrange
        var request = new BaseFilterRequest
        {
            ColumnName = columnName,
            SortOrder = sortOrder,
        };

        var spec = new TestSpecification(request);

        // Act
        var result = spec.Evaluate(_testCollection).ToList();

        // Assert
        result
            .Should()
            .HaveCount(5)
            .And
            .BeInAscendingOrder(x => x.CreatedOn);
    }

    [Theory]
    [MemberData(nameof(LastModifiedOnTestData))]
    public void ApplyDefaultOrdering_SortsByLastModifiedOn(string columnName, SortOrder? sortOrder)
    {
        // Arrange
        var request = new BaseFilterRequest
        {
            ColumnName = columnName,
            SortOrder = sortOrder,
        };

        var spec = new TestSpecification(request);

        // Act
        var result = spec.Evaluate(_testCollection).ToList();

        // Assert
        result
            .Should()
            .HaveCount(5)
            .And
            .BeInAscendingOrder(x => x.LastModifiedBy);
    }


    public static IEnumerable<object[]> CreatedOnSortTestData =>
        new List<object[]>
        {
            new object[] { "CreatedOn", SortOrder.Asc},
            new object[] { "createdOn", SortOrder.Asc},

            new object[] { "CreatedOn", null},
            new object[] { "createdOn", null},
        };

    public static IEnumerable<object[]> LastModifiedOnTestData =>
        new List<object[]>
        {
            new object[] { "LastModifiedOn", SortOrder.Asc},
            new object[] { "lastModifiedOn", SortOrder.Asc },

            new object[] { "LastModifiedOn", null},
            new object[] { "lastModifiedOn", null},
        };
}
