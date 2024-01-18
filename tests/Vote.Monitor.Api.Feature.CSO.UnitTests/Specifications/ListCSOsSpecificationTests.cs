namespace Vote.Monitor.Api.Feature.CSO.UnitTests.Specifications;

public class ListCSOsSpecificationTests
{
    private const string DefaultName = "name";
    private readonly CSOStatus DefaultStatus = CSOStatus.Activated;

    [Fact]
    public void ListCSOsSpecification_PaginatesCorrectly()
    {
        // Arrange
        var cso1 = new CSOAggregateFaker(index: 101, status: DefaultStatus).Generate();
        var cso2 = new CSOAggregateFaker(index: 102, status: DefaultStatus).Generate();

        var testCollection = Enumerable.Range(1, 100)
            .Select(idx => new CSOAggregateFaker(index: idx, status: DefaultStatus).Generate())
        .Union(new[] { cso1, cso2 })
        .ToList();

        var request = new List.Request
        {
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListCSOsSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(cso1);
        result.Should().Contain(cso2);
    }

    [Fact]
    public void ListCSOsSpecification_AppliesCorrectFilters_WhenNameFilterApplied()
    {
        // Arrange
        var cso1 = new CSOAggregateFaker(index: 101, name: DefaultName, status: DefaultStatus).Generate();
        var cso2 = new CSOAggregateFaker(index: 102, name: DefaultName, status: DefaultStatus).Generate();

        var testCollection = Enumerable
        .Range(1, 100)
            .Select(index => new CSOAggregateFaker(index: index, status: DefaultStatus).Generate())
            .Union(new[] { cso1, cso2 })
            .ToList();

        var request = new List.Request
        {
            NameFilter = DefaultName,
        };

        var spec = new ListCSOsSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(cso1);
        result.Should().Contain(cso2);
    }

    [Fact]
    public void ListCSOsSpecification_AppliesCorrectFilters_WhenStatusFilterApplied()
    {
        // Arrange
        var cso1 = new CSOAggregateFaker(index: 101, name: DefaultName, status: CSOStatus.Activated).Generate();
        var cso2 = new CSOAggregateFaker(index: 102, name: DefaultName, status: CSOStatus.Activated).Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(index => new CSOAggregateFaker(index: index, status: CSOStatus.Deactivated).Generate())
            .Union(new[] { cso1, cso2 })
            .ToList();

        var request = new List.Request
        {
            Status = CSOStatus.Activated,
        };

        var spec = new ListCSOsSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(cso1);
        result.Should().Contain(cso2);
    }

    [Theory]
    [InlineData("name1")]
    [InlineData("name2")]
    public void ListCSOsSpecification_AppliesCorrectFilters_WhenPartialFilterApplied(string searchString)
    {
        // Arrange
        var cso1 = new CSOAggregateFaker(index: 101, name: searchString, status: DefaultStatus).Generate();
        var cso2 = new CSOAggregateFaker(index: 102, name: searchString, status: DefaultStatus).Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(idx => new CSOAggregateFaker(index: idx, name: searchString, status: DefaultStatus).Generate())
            .Union(new[] { cso1, cso2 })
            .ToList();

        var request = new List.Request
        {
            NameFilter = searchString,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListCSOsSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(cso1);
        result.Should().Contain(cso2);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void ListCSOsSpecification_AppliesDefaultSorting_WhenNoSortColumnSet(string columnName)
    {
        // Arrange
        var cso1 = new CSOAggregateFaker(index: 101).Generate();
        var cso2 = new CSOAggregateFaker(index: 102).Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(idx => new CSOAggregateFaker(index: idx, status: DefaultStatus).Generate())
            .Union(new[] { cso1, cso2 })
            .ToList();

        var request = new List.Request
        {
            SortColumnName = columnName,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListCSOsSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(cso1);
        result.Should().Contain(cso2);
    }

    [Theory]
    [MemberData(nameof(NameSortingTestCases))]
    public void ListCSOsSpecification_AppliesSortingCorrectly(string columnName, SortOrder? sortOrder)
    {
        // Arrange
        var cso1 = new CSOAggregateFaker(index: 1, name: "cso-901").Generate();
        var cso2 = new CSOAggregateFaker(index: 2, name: "cso-902").Generate();

        var testCollection = Enumerable
            .Range(100, 100)
            .Select(idx => new CSOAggregateFaker(index: idx, name: $"cso-{idx}").Generate())
            .Union(new[] { cso1, cso2 })
            .Reverse()
            .ToList();

        var request = new List.Request
        {
            SortColumnName = columnName,
            SortOrder = sortOrder,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListCSOsSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(cso1);
        result.Should().Contain(cso2);
    }

    [Fact]
    public void ListCSOsSpecification_AppliesSortOrderCorrectly()
    {
        // Arrange
        var cso1 = new CSOAggregateFaker(index: 1, name: "cso-101").Generate();
        var cso2 = new CSOAggregateFaker(index: 2, name: "cso-102").Generate();

        var testCollection = Enumerable
            .Range(900, 100)
            .Select(idx => new CSOAggregateFaker(index: idx, name: $"cso-{idx}").Generate())
            .Union(new[] { cso1, cso2 })
            .Reverse()
            .ToList();

        var request = new List.Request
        {
            SortColumnName = "name",
            SortOrder = SortOrder.Desc,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListCSOsSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(cso1);
        result.Should().Contain(cso2);
    }
    public static IEnumerable<object[]> NameSortingTestCases =>
        new List<object[]>
        {
            new object[] { "name", null },
            new object[] { "Name", null },
            new object[] { "name", SortOrder.Asc },
            new object[] { "Name", SortOrder.Asc }
        };
}
