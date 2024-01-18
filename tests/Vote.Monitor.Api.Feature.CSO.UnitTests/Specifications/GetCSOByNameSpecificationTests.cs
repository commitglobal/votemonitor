namespace Vote.Monitor.Api.Feature.CSO.UnitTests.Specifications;

public class GetCSOByNameSpecificationTests
{
    [Fact]
    public void GetCSOByNameSpecification_UsesExactMatch()
    {
        // Arrange
        var cso = new CSOAggregateFaker(name: "My little CSO").Generate();

        var testCollection = new CSOAggregateFaker()
            .Generate(500)
            .Union(new[] { cso })
            .Union(new CSOAggregateFaker().Generate(500))
            .ToList();

        var spec = new GetCSOByNameSpecification(cso.Name);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1); // Expecting only one item in the result
        result.Should().Contain(cso);
    }

    [Fact]
    public void GetCSOByNameSpecification_DoesNotMatchPartialName()
    {
        // Arrange
        var cso = new CSOAggregateFaker(name: "a weird cso name").Generate();

        var testCollection = new CSOAggregateFaker()
            .Generate(500)
            .Union(new[] { cso })
            .Union(new CSOAggregateFaker().Generate(500))
            .ToList();

        var spec = new GetCSOByNameSpecification("a weird cso");

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(0); // Expecting none
    }
}
