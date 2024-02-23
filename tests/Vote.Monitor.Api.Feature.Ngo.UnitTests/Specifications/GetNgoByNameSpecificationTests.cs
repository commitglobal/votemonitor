using Vote.Monitor.Api.Feature.Ngo.Specifications;
using Vote.Monitor.TestUtils.Fakes;

namespace Vote.Monitor.Api.Feature.Ngo.UnitTests.Specifications;

public class GetNgoByNameSpecificationTests
{
    [Fact]
    public void GetNgoByNameSpecification_UsesExactMatch()
    {
        // Arrange
        var ngo = new NgoAggregateFaker(name: "My little Ngo").Generate();

        var testCollection = new NgoAggregateFaker()
            .Generate(500)
            .Union(new[] { ngo })
            .Union(new NgoAggregateFaker().Generate(500))
            .ToList();

        var spec = new GetNgoByNameSpecification(ngo.Name);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1); // Expecting only one item in the result
        result.Should().Contain(ngo);
    }

    [Fact]
    public void GetNgoByNameSpecification_DoesNotMatchPartialName()
    {
        // Arrange
        var ngo = new NgoAggregateFaker(name: "a weird ngo name").Generate();

        var testCollection = new NgoAggregateFaker()
            .Generate(500)
            .Union(new[] { ngo })
            .Union(new NgoAggregateFaker().Generate(500))
            .ToList();

        var spec = new GetNgoByNameSpecification("a weird ngo");

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(0); // Expecting none
    }
}
