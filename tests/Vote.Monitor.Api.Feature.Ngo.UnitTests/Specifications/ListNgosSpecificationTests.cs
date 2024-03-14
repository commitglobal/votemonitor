
using Vote.Monitor.Api.Feature.Ngo.List;
using Vote.Monitor.Api.Feature.Ngo.Specifications;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.NgoAggregate;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Api.Feature.Ngo.UnitTests.Specifications;

public class ListNgosSpecificationTests
{
    private const string DefaultName = "name";
    private readonly NgoStatus DefaultStatus = NgoStatus.Activated;

    [Fact]
    public void ListNgosSpecification_PaginatesCorrectly()
    {
        // Arrange
        var ngo1 = new NgoAggregateFaker(index: 101, status: DefaultStatus).Generate();
        var ngo2 = new NgoAggregateFaker(index: 102, status: DefaultStatus).Generate();

        var testCollection = Enumerable.Range(1, 100)
            .Select(idx => new NgoAggregateFaker(index: idx, status: DefaultStatus).Generate())
        .Union(new[] { ngo1, ngo2 })
        .ToList();

        var request = new Request
        {
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListNgosSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(ngo1);
        result.Should().Contain(ngo2);
    }

    [Fact]
    public void ListNgosSpecification_AppliesCorrectFilters_WhenNameFilterApplied()
    {
        // Arrange
        var ngo1 = new NgoAggregateFaker(index: 101, name: DefaultName, status: DefaultStatus).Generate();
        var ngo2 = new NgoAggregateFaker(index: 102, name: DefaultName, status: DefaultStatus).Generate();

        var testCollection = Enumerable
        .Range(1, 100)
            .Select(index => new NgoAggregateFaker(index: index, status: DefaultStatus).Generate())
            .Union(new[] { ngo1, ngo2 })
            .ToList();

        var request = new Request
        {
            NameFilter = DefaultName
        };

        var spec = new ListNgosSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(ngo1);
        result.Should().Contain(ngo2);
    }

    [Fact]
    public void ListNgosSpecification_AppliesCorrectFilters_WhenStatusFilterApplied()
    {
        // Arrange
        var ngo1 = new NgoAggregateFaker(index: 101, name: DefaultName, status: NgoStatus.Activated).Generate();
        var ngo2 = new NgoAggregateFaker(index: 102, name: DefaultName, status: NgoStatus.Activated).Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(index => new NgoAggregateFaker(index: index, status: NgoStatus.Deactivated).Generate())
            .Union(new[] { ngo1, ngo2 })
            .ToList();

        var request = new Request
        {
            Status = NgoStatus.Activated
        };

        var spec = new ListNgosSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(ngo1);
        result.Should().Contain(ngo2);
    }

    [Theory]
    [InlineData("name1")]
    [InlineData("name2")]
    public void ListNgosSpecification_AppliesCorrectFilters_WhenPartialFilterApplied(string searchString)
    {
        // Arrange
        var ngo1 = new NgoAggregateFaker(index: 101, name: searchString, status: DefaultStatus).Generate();
        var ngo2 = new NgoAggregateFaker(index: 102, name: searchString, status: DefaultStatus).Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(idx => new NgoAggregateFaker(index: idx, name: searchString, status: DefaultStatus).Generate())
            .Union(new[] { ngo1, ngo2 })
            .ToList();

        var request = new Request
        {
            NameFilter = searchString,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListNgosSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(ngo1);
        result.Should().Contain(ngo2);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void ListNgosSpecification_AppliesDefaultSorting_WhenNoSortColumnSet(string columnName)
    {
        // Arrange
        var ngo1 = new NgoAggregateFaker(index: 101).Generate();
        var ngo2 = new NgoAggregateFaker(index: 102).Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(idx => new NgoAggregateFaker(index: idx, status: DefaultStatus).Generate())
            .Union(new[] { ngo1, ngo2 })
            .ToList();

        var request = new Request
        {
            SortColumnName = columnName,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListNgosSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(ngo1);
        result.Should().Contain(ngo2);
    }

    [Theory]
    [MemberData(nameof(NameSortingTestCases))]
    public void ListNgosSpecification_AppliesSortingCorrectly(string columnName, SortOrder? sortOrder)
    {
        // Arrange
        var ngo1 = new NgoAggregateFaker(index: 1, name: "ngo-901").Generate();
        var ngo2 = new NgoAggregateFaker(index: 2, name: "ngo-902").Generate();

        var testCollection = Enumerable
            .Range(100, 100)
            .Select(idx => new NgoAggregateFaker(index: idx, name: $"ngo-{idx}").Generate())
            .Union(new[] { ngo1, ngo2 })
            .Reverse()
            .ToList();

        var request = new Request
        {
            SortColumnName = columnName,
            SortOrder = sortOrder,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListNgosSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(ngo1);
        result.Should().Contain(ngo2);
    }

    [Fact]
    public void ListNgosSpecification_AppliesSortOrderCorrectly()
    {
        // Arrange
        var ngo1 = new NgoAggregateFaker(index: 1, name: "ngo-101").Generate();
        var ngo2 = new NgoAggregateFaker(index: 2, name: "ngo-102").Generate();

        var testCollection = Enumerable
            .Range(900, 100)
            .Select(idx => new NgoAggregateFaker(index: idx, name: $"ngo-{idx}").Generate())
            .Union(new[] { ngo1, ngo2 })
            .Reverse()
            .ToList();

        var request = new Request
        {
            SortColumnName = "name",
            SortOrder = SortOrder.Desc,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListNgosSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(ngo1);
        result.Should().Contain(ngo2);
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
