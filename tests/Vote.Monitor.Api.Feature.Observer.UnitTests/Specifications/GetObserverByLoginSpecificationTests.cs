namespace Vote.Monitor.Api.Feature.Observer.Specifications;

public class GetObserverByLoginSpecificationTests
{
    [Fact]
    public void GetObserverByLoginSpecificationTests_AppliesCorrectFilters()
    {
        // Arrange
        var observer = new ObserverAggregateFaker().Generate();

        var testCollection = new ObserverAggregateFaker()
            .Generate(500)
            .Union(new[] { observer })
            .Union(new ObserverAggregateFaker().Generate(500))
            .ToList();

        var spec = new GetObserverByLoginSpecification(observer.Login);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        //// Assert
        result.Should().HaveCount(1); // Expecting only one item in the result
        result.Should().Contain(observer);
    }
}
