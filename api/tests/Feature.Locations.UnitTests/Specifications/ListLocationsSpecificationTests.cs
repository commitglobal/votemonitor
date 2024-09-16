using Feature.Locations.List;
using Vote.Monitor.Core.Models;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.Locations.UnitTests.Specifications;

public class ListLocationsSpecificationTests
{
    [Fact]
    public void ListLocationSpecification_AppliesCorrectFilters()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var location1 = new LocationAggregateFaker(electionRound: electionRound, displayOrder: 101).Generate();
        var location2 = new LocationAggregateFaker(electionRound: electionRound, displayOrder: 102).Generate();

        var testCollection = Enumerable.Range(1, 100)
            .Select(displayOrder =>
                new LocationAggregateFaker(electionRound: electionRound, displayOrder: displayOrder).Generate())
            .Union(new[] { location1, location2 })
            .ToList();

        var request = new Request
        {
            ElectionRoundId = electionRound.Id,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListLocationsSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(location1);
        result.Should().Contain(location2);
    }

    [Fact]
    public void ListLocationsSpecification_AppliesCorrectFilters_WhenAddressFilterApplied()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var location1 =
            new LocationAggregateFaker(electionRound: electionRound, displayOrder: 101)
                .Generate();
        var location2 =
            new LocationAggregateFaker(electionRound: electionRound, displayOrder: 102)
                .Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(displayOrder =>
                new LocationAggregateFaker(electionRound: electionRound, displayOrder: displayOrder).Generate())
            .Union(new[] { location1, location2 })
            .ToList();

        var request = new Request
        {
            ElectionRoundId = electionRound.Id,
            PageSize = 100,
            PageNumber = 2
        };
        var spec = new ListLocationsSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(location1);
        result.Should().Contain(location2);
    }

    [Theory]
    [InlineData("Fluwelen Burgwal 58")]
    [InlineData("2511 CJ Den Haag")]
    public void ListPollingStationsSpecification_AppliesCorrectFilters_WhenPartialAddressFilterApplied(
        string searchString)
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var pollingStation1 =
            new LocationAggregateFaker(electionRound: electionRound, displayOrder: 101)
                .Generate();
        var pollingStation2 =
            new LocationAggregateFaker(electionRound: electionRound, displayOrder: 102)
                .Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(displayOrder =>
                new LocationAggregateFaker(electionRound: electionRound, displayOrder: displayOrder).Generate())
            .Union(new[] { pollingStation1, pollingStation2 })
            .ToList();

        var request = new Request
        {
            ElectionRoundId = electionRound.Id,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListLocationsSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(pollingStation1);
        result.Should().Contain(pollingStation2);
    }


    [Theory]
    [MemberData(nameof(SortingTestCases))]
    public void ListPollingStationsSpecification_AppliesSortingCorrectly(string columnName, SortOrder? sortOrder)
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker().Generate();
        var pollingStation1 =
            new LocationAggregateFaker(electionRound: electionRound, displayOrder: 102)
                .Generate();
        var pollingStation2 =
            new LocationAggregateFaker(electionRound: electionRound, displayOrder: 103)
                .Generate();

        var testCollection = Enumerable
            .Range(1, 100)
            .Select(displayOrder =>
                new LocationAggregateFaker(electionRound: electionRound, displayOrder: displayOrder).Generate())
            .Union(new[] { pollingStation1, pollingStation2 })
            .Reverse()
            .ToList();

        var request = new Request
        {
            ElectionRoundId = electionRound.Id,
            SortOrder = sortOrder,
            SortColumnName = columnName,
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListLocationsSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(pollingStation1);
        result.Should().Contain(pollingStation2);
    }

    public static IEnumerable<object[]> SortingTestCases =>
        new List<object[]>
        {
            new object[] { "name", null },
            new object[] { "Name", null },
            new object[] { "displayOrder", null },
            new object[] { "name", SortOrder.Asc },
            new object[] { "Name", SortOrder.Asc },
            new object[] { "DisplayOrder", SortOrder.Asc }
        };
}