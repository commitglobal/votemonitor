using Vote.Monitor.Api.Feature.Observer.Specifications;
using Vote.Monitor.Core.Models;
using Vote.Monitor.TestUtils;

namespace Vote.Monitor.Api.Feature.Observer.UnitTests.Specifications;

public class ListObserversSpecificationTests
{
    private const string DefaultName = "name";
    private readonly UserStatus DefaultStatus = UserStatus.Active;

    [Fact]
    public void ListObserversSpecification_AppliesCorrectFilters()
    {
        // Arrange
        var observer1 = new ObserverAggregateFaker(index: 101, status: DefaultStatus).Generate();
        var observer2 = new ObserverAggregateFaker(index: 102, status: DefaultStatus).Generate();

        var testCollection = Enumerable.Range(1, 100)
            .Select(idx => new ObserverAggregateFaker(index: idx, status: DefaultStatus).Generate())
        .Union(new[] { observer1, observer2 })
        .ToList();

        var request = new List.Request
        {
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListObserversSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(observer1);
        result.Should().Contain(observer2);
    }

    [Fact]
    public void ListObserversSpecification_AppliesCorrectFilters_WhenNameFilterApplied()
    {
        // Arrange
        var observer1 = new ObserverAggregateFaker(index: 101, name: DefaultName, status: DefaultStatus).Generate();
        var observer2 = new ObserverAggregateFaker(index: 102, name: DefaultName, status: DefaultStatus).Generate();

        var testCollection = Enumerable
        .Range(1, 100)
            .Select(index => new ObserverAggregateFaker(index: index, name: DefaultName, status: DefaultStatus).Generate())
            .Union(new[] { observer1, observer2 })
            .ToList();

        var request = new List.Request
        {
            NameFilter = DefaultName,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListObserversSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(observer1);
        result.Should().Contain(observer2);
    }

    [Theory]
    [InlineData("name1")]
    [InlineData("name2")]
    public void ListObserversSpecification_AppliesCorrectFilters_WhenPartialFilterApplied(string searchString)
    {
        // Arrange
        var observer1 = new ObserverAggregateFaker(index: 101, name: searchString, status: DefaultStatus).Generate();
        var observer2 = new ObserverAggregateFaker(index: 102, name: searchString, status: DefaultStatus).Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(idx => new ObserverAggregateFaker(index: idx, name: searchString, status: DefaultStatus).Generate())
            .Union(new[] { observer1, observer2 })
            .ToList();

        var request = new List.Request
        {
            NameFilter = searchString,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListObserversSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(observer1);
        result.Should().Contain(observer2);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void ListObserversSpecification_AppliesDefaultSorting_WhenNoSortColumnSet(string columnName)
    {
        // Arrange
        var observer1 = new ObserverAggregateFaker(index: 101).Generate();
        var observer2 = new ObserverAggregateFaker(index: 102).Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(idx => new ObserverAggregateFaker(index: idx, status: DefaultStatus).Generate())
            .Union(new[] { observer1, observer2 })
            .ToList();

        var request = new List.Request
        {
            ColumnName = columnName,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListObserversSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(observer1);
        result.Should().Contain(observer2);
    }

    [Theory]
    [MemberData(nameof(NameSortingTestCases))]
    public void ListObserversSpecification_AppliesSortingCorrectly(string columnName, SortOrder? sortOrder)
    {
        // Arrange
        var observer1 = new ObserverAggregateFaker(index: 1, name: "Observer-901").Generate();
        var observer2 = new ObserverAggregateFaker(index: 2, name: "Observer-902").Generate();

        var testCollection = Enumerable
            .Range(100, 100)
            .Select(idx => new ObserverAggregateFaker(index: idx, name: $"Observer-{idx}").Generate())
            .Union(new[] { observer1, observer2 })
            .Reverse()
            .ToList();

        var request = new List.Request
        {
            ColumnName = columnName,
            SortOrder = sortOrder,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListObserversSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(observer1);
        result.Should().Contain(observer2);
    }

    [Fact]
    public void ListObserversSpecification_AppliesSortOrderCorrectly()
    {
        // Arrange
        var observer1 = new ObserverAggregateFaker(index: 1, name: "Observer-101").Generate();
        var observer2 = new ObserverAggregateFaker(index: 2, name: "Observer-102").Generate();

        var testCollection = Enumerable
            .Range(900, 100)
            .Select(idx => new ObserverAggregateFaker(index: idx, name: $"Observer-{idx}").Generate())
            .Union(new[] { observer1, observer2 })
            .Reverse()
            .ToList();

        var request = new List.Request
        {
            ColumnName = "name",
            SortOrder = SortOrder.Desc,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListObserversSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(observer1);
        result.Should().Contain(observer2);
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
