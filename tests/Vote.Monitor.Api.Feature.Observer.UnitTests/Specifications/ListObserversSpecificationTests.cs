

namespace Vote.Monitor.Api.Feature.Observer.Specifications;

public class ListObserversSpecificationTests
{
    private const string DefaultName = "name";
    private readonly UserStatus DefaultStatus = UserStatus.Active;

    [Fact]
    public void ListObserversSpecification_AppliesCorrectFilters()
    {
        // Arrange
        var observer1 = new ObserverAggregateFaker(status: DefaultStatus).Generate();
        var observer2 = new ObserverAggregateFaker(status: DefaultStatus).Generate();

        var testCollection = Enumerable.Range(1, 100)
            .Select(status => new ObserverAggregateFaker(status: DefaultStatus).Generate())
        .Union(new[] { observer1, observer2 })
        .ToList();

        var spec = new ListObserversSpecification(null, null, 100, 2);

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
        var observer1 = new ObserverAggregateFaker(name: DefaultName, status: DefaultStatus).Generate();
        var observer2 = new ObserverAggregateFaker(name: DefaultName, status: DefaultStatus).Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(statusArg => new ObserverAggregateFaker(name: DefaultName, status: DefaultStatus).Generate())
            .Union(new[] { observer1, observer2 })
            .ToList();

        var spec = new ListObserversSpecification(DefaultName, null, 100, 2);

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
        var observer1 = new ObserverAggregateFaker(name: searchString, status: DefaultStatus).Generate();
        var observer2 = new ObserverAggregateFaker(name: searchString, status: DefaultStatus).Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(statusArg => new ObserverAggregateFaker(name: searchString, status: DefaultStatus).Generate())
            .Union(new[] { observer1, observer2 })
            .ToList();

        var spec = new ListObserversSpecification(searchString, null, 100, 2);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(observer1);
        result.Should().Contain(observer2);
    }
}
