using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.Feature.Monitoring.UnitTests.Specifications;

public class GetNgoStatusSpecificationTests
{
    [Theory]
    [MemberData(nameof(NGOStatuses))]
    public void ShouldMatch_NgoById(NgoStatus status)
    {
        // Arrange
        var ngoId = Guid.NewGuid();
        var ngo = new NgoAggregateFaker(ngoId, status: status).Generate();

        List<NgoAggregate> testCollection =
        [
            ngo,
            .. new NgoAggregateFaker().Generate(100)
        ];

        // Act
        var spec = new GetNgoStatusSpecification(ngoId);
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(status);
    }

    [Fact]
    public void ShouldNotMatchAny_WhenNoSuchNgo()
    {
        // Arrange
        var ngoId = Guid.NewGuid();
        List<NgoAggregate> testCollection =
        [
            .. new NgoAggregateFaker().Generate(100)
        ];

        // Act
        var spec = new GetNgoStatusSpecification(ngoId);
        var result = spec.Evaluate(testCollection).FirstOrDefault();

        // Assert
        result.Should().BeNull();
    }

    public static IEnumerable<object[]> NGOStatuses =>
        new List<object[]>
        {
            new object[] { NgoStatus.Activated },
            new object[] { NgoStatus.Deactivated }
        };
}
