using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.Observer.UnitTests.Specifications;

public class ListObserversSpecificationTests
{
    private const string DefaultName = "Default$ObserverName";

    [Fact]
    public void ListObserversSpecification_AppliesCorrectFilters()
    {
        // Arrange
        var observer1 = new ObserverAggregateFaker(index: 101).Generate();
        var observer2 = new ObserverAggregateFaker(index: 102).Generate();

        var testCollection = Enumerable.Range(1, 100)
            .Select(idx => new ObserverAggregateFaker(index: idx).Generate())
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
        var observer1 = new ObserverAggregateFaker(index: 101, applicationUser: new ApplicationUserFaker(name: DefaultName)).Generate();
        var observer2 = new ObserverAggregateFaker(index: 102, applicationUser: new ApplicationUserFaker(name: DefaultName)).Generate();

        var testCollection = Enumerable
        .Range(1, 100)
            .Select(index => new ObserverAggregateFaker(index: index).Generate())
            .Union(new[] { observer1, observer2 })
            .ToList();

        var request = new List.Request
        {
            SearchText = DefaultName
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
        var observer1 = new ObserverAggregateFaker(index: 101, applicationUser: new ApplicationUserFaker(name: searchString)).Generate();
        var observer2 = new ObserverAggregateFaker(index: 102, applicationUser: new ApplicationUserFaker(name: searchString)).Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(idx => new ObserverAggregateFaker(index: idx).Generate())
            .Union(new[] { observer1, observer2 })
            .ToList();

        var request = new List.Request
        {
            SearchText = searchString
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
            .Select(idx => new ObserverAggregateFaker(index: idx).Generate())
            .Union(new[] { observer1, observer2 })
            .ToList();

        var request = new List.Request
        {
            SortColumnName = columnName,
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
        var observer1 = new ObserverAggregateFaker(index: 1, applicationUser: new ApplicationUserFaker(name: "Observer-901")).Generate();
        var observer2 = new ObserverAggregateFaker(index: 2, applicationUser: new ApplicationUserFaker(name: "Observer-902")).Generate();

        var testCollection = Enumerable
            .Range(100, 100)
            .Select(idx => new ObserverAggregateFaker(index: idx, applicationUser: new ApplicationUserFaker(name: $"Observer-{idx}")).Generate())
            .Union(new[] { observer1, observer2 })
            .Reverse()
            .ToList();

        var request = new List.Request
        {
            SortColumnName = columnName,
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
        var observer1 = new ObserverAggregateFaker(index: 1, applicationUser: new ApplicationUserFaker(name: "Observer-101")).Generate();
        var observer2 = new ObserverAggregateFaker(index: 2, applicationUser: new ApplicationUserFaker(name: "Observer-102")).Generate();

        var testCollection = Enumerable
            .Range(900, 100)
            .Select(idx => new ObserverAggregateFaker(index: idx, applicationUser: new ApplicationUserFaker(name: $"Observer-{idx}")).Generate())
            .Union(new[] { observer1, observer2 })
            .Reverse()
            .ToList();

        var request = new List.Request
        {
            SortColumnName = "FirstName",
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
            new object[] { "firstName", null },
            new object[] { "FirstName", null },
            new object[] { "firstName", SortOrder.Asc },
            new object[] { "FirstName", SortOrder.Asc }
        };
}
