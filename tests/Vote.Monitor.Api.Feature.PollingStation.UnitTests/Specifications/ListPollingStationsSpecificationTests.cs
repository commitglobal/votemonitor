using Vote.Monitor.Api.Feature.PollingStation.List;

namespace Vote.Monitor.Api.Feature.PollingStation.UnitTests.Specifications;

public class ListPollingStationsSpecificationTests
{
    private const string DefaultAddress = "Fluwelen Burgwal 58, 2511 CJ Den Haag";

    [Fact]
    public void ListPollingStationsSpecification_AppliesCorrectFilters()
    {
        // Arrange
        var pollingStation1 = new PollingStationAggregateFaker(displayOrder: 101).Generate();
        var pollingStation2 = new PollingStationAggregateFaker(displayOrder: 102).Generate();

        var testCollection = Enumerable.Range(1, 100)
            .Select(displayOrder => new PollingStationAggregateFaker(displayOrder: displayOrder).Generate())
            .Union(new[] { pollingStation1, pollingStation2 })
            .ToList();

        var request = new Request
        {
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListPollingStationsSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(pollingStation1);
        result.Should().Contain(pollingStation2);
    }

    [Fact]
    public void ListPollingStationsSpecification_AppliesCorrectFilters_WhenAddressFilterApplied()
    {
        // Arrange
        var pollingStation1 = new PollingStationAggregateFaker(displayOrder: 101, address: DefaultAddress).Generate();
        var pollingStation2 = new PollingStationAggregateFaker(displayOrder: 102, address: DefaultAddress).Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(displayOrder => new PollingStationAggregateFaker(displayOrder: displayOrder, address: DefaultAddress).Generate())
            .Union(new[] { pollingStation1, pollingStation2 })
            .ToList();

        var request = new Request
        {
            AddressFilter = DefaultAddress,
            PageSize = 100,
            PageNumber = 2
        };
        var spec = new ListPollingStationsSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(pollingStation1);
        result.Should().Contain(pollingStation2);
    }

    [Theory]
    [InlineData("Fluwelen Burgwal 58")]
    [InlineData("2511 CJ Den Haag")]
    public void ListPollingStationsSpecification_AppliesCorrectFilters_WhenPartialAddressFilterApplied(string searchString)
    {
        // Arrange
        var pollingStation1 = new PollingStationAggregateFaker(displayOrder: 101, address: DefaultAddress).Generate();
        var pollingStation2 = new PollingStationAggregateFaker(displayOrder: 102, address: DefaultAddress).Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(displayOrder => new PollingStationAggregateFaker(displayOrder: displayOrder, address: DefaultAddress).Generate())
            .Union(new[] { pollingStation1, pollingStation2 })
            .ToList();

        var request = new Request
        {
            AddressFilter = searchString,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListPollingStationsSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(pollingStation1);
        result.Should().Contain(pollingStation2);
    }
}
