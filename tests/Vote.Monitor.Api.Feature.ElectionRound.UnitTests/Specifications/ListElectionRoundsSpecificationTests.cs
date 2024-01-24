
namespace Vote.Monitor.Api.Feature.ElectionRound.UnitTests.Specifications;

public class ListElectionRoundsSpecificationTests
{
    private const string DefaultElectionRoundTitle = "A big election 2024";

    [Theory]
    [MemberData(nameof(TitleFilters))]
    public void ShouldFilterByTitle_WhenTitleFilterNotEmpty(string titleFilter)
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker(title: DefaultElectionRoundTitle).Generate();

        List<ElectionRoundAggregate> testCollection =
        [
            electionRound,
            .. new ElectionRoundAggregateFaker().Generate(100)
        ];

        // Act
        List.Request request = new List.Request
        {
            TitleFilter = titleFilter
        };
        var spec = new ListElectionRoundsSpecification(request);
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.First().Should().BeEquivalentTo(electionRound, cfg=>cfg.ExcludingMissingMembers());
    }

    [Fact]
    public void ShouldFilterByCountry_WhenCountryFilterNotEmpty()
    {
        // Arrange
        var countryId = CountriesList.MD.Id;
        var electionRound = new ElectionRoundAggregateFaker(countryId: countryId).Generate();

        List<ElectionRoundAggregate> testCollection =
        [
            electionRound,
            .. new ElectionRoundAggregateFaker().Generate(100)
        ];

        // Act
        List.Request request = new List.Request
        {
            CountryId = countryId
        };
        var spec = new ListElectionRoundsSpecification(request);
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.First().Should().BeEquivalentTo(electionRound, cfg => cfg.ExcludingMissingMembers());
    }

    [Fact]
    public void ShouldFilterByStatus_WhenStatusFilterNotEmpty()
    {
        // Arrange
        var electionRound = new ElectionRoundAggregateFaker(status: ElectionRoundStatus.Started).Generate();

        List<ElectionRoundAggregate> testCollection =
        [
            electionRound,
            new ElectionRoundAggregateFaker(status: ElectionRoundStatus.NotStarted).Generate(),
            new ElectionRoundAggregateFaker(status: ElectionRoundStatus.Archived).Generate()
        ];

        // Act
        List.Request request = new List.Request
        {
            Status = ElectionRoundStatus.Started
        };
        var spec = new ListElectionRoundsSpecification(request);
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().HaveCount(1);
        result.First().Should().BeEquivalentTo(electionRound, cfg => cfg.ExcludingMissingMembers());
    }

    [Theory]
    [MemberData(nameof(TitleSortingTestCases))]
    public void ShouldSortByTitle_WhenSortColumnTitle(string sortColumn, SortOrder sortOrder)
    {
        // Arrange
        var electionRound1 = new ElectionRoundAggregateFaker(title: "Election title 1").Generate();
        var electionRound2 = new ElectionRoundAggregateFaker(title: "Election title 2").Generate();
        var electionRound3 = new ElectionRoundAggregateFaker(title: "Election title 3").Generate();
        var electionRound4 = new ElectionRoundAggregateFaker(title: "Election title 4").Generate();

        List<ElectionRoundAggregate> testCollection =
        [
            electionRound4,
            electionRound2,
            electionRound3,
            electionRound1
        ];

        // Act
        List.Request request = new List.Request
        {
            SortColumnName = sortColumn,
            SortOrder = sortOrder
        };
        var spec = new ListElectionRoundsSpecification(request);
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(4);
        result[0].Should().BeEquivalentTo(electionRound1, cfg => cfg.ExcludingMissingMembers());
        result[1].Should().BeEquivalentTo(electionRound2, cfg => cfg.ExcludingMissingMembers());
        result[2].Should().BeEquivalentTo(electionRound3, cfg => cfg.ExcludingMissingMembers());
        result[3].Should().BeEquivalentTo(electionRound4, cfg => cfg.ExcludingMissingMembers());
    }

    // For now sorting by status is a known issue. It only occurs for in-memory collections.
    // Issue: https://github.com/ardalis/Specification/issues/383
    //[Theory]
    //[MemberData(nameof(StatusSortingTestCases))]
    //public void ShouldSortByStatus_WhenSortColumnStatus(string sortColumn, SortOrder sortOrder)
    //{
    //    // Arrange
    //    var electionRound1 = new ElectionRoundFaker(status: ElectionRoundStatus.Started).Generate();
    //    var electionRound2 = new ElectionRoundFaker(status: ElectionRoundStatus.Archived).Generate();
    //    var electionRound3 = new ElectionRoundFaker(status: ElectionRoundStatus.NotStarted).Generate();

    //    List<ElectionRoundAggregate> testCollection =
    //    [
    //        electionRound2,
    //        electionRound3,
    //        electionRound1
    //    ];

    //    // Act
    //    List.Request request = new List.Request
    //    {
    //        SortColumnName = sortColumn,
    //        SortOrder = sortOrder
    //    };
    //    var spec = new ListElectionRoundsSpecification(request);
    //    var result = spec.Evaluate(testCollection).ToList();

    //    // Assert
    //    result.Should().HaveCount(3);
    //    result.Should().BeInAscendingOrder(x => x.Status);
    //}

    [Fact]
    public void ShouldApplyDefaultSorting_WhenNoSortingRequested()
    {
        // Arrange
        var electionRound1 = new ElectionRoundAggregateFaker(index: 3, title: "Election title").Generate();
        var electionRound2 = new ElectionRoundAggregateFaker(index: 2, title: "Election title").Generate();
        var electionRound3 = new ElectionRoundAggregateFaker(index: 1, title: "Election title").Generate();

        List<ElectionRoundAggregate> testCollection =
        [
            electionRound2,
            electionRound3,
            electionRound1
        ];

        // Act
        List.Request request = new List.Request();
        var spec = new ListElectionRoundsSpecification(request);
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(3);
        result
            .Should()
            .BeInAscendingOrder(x => x.CreatedOn)
            .And.BeInAscendingOrder(x => x.Title);
    }

    [Fact]
    public void ShouldPaginateCorrectly()
    {
        // Arrange
        var electionRound1 = new ElectionRoundAggregateFaker(index: 101).Generate();
        var electionRound2 = new ElectionRoundAggregateFaker(index: 102).Generate();

        var testCollection = Enumerable.Range(1, 100)
            .Select(idx => new ElectionRoundAggregateFaker(index: idx).Generate())
            .Union(new[] { electionRound1, electionRound2 })
            .ToList();

        var request = new List.Request
        {
            PageSize = 100,
            PageNumber = 2
        };

        var spec = new ListElectionRoundsSpecification(request);

        // Act
        var result = spec.Evaluate(testCollection).ToList();

        // Assert
        result.Should().HaveCount(2);

        result[0].Should().BeEquivalentTo(electionRound1, cfg => cfg.ExcludingMissingMembers());
        result[1].Should().BeEquivalentTo(electionRound2, cfg => cfg.ExcludingMissingMembers());
    }

    public static IEnumerable<object[]> TitleFilters =>
        new List<object[]>
        {
            new object[] { DefaultElectionRoundTitle },
            new object[] { "A big election" }
        };

    public static IEnumerable<object[]> TitleSortingTestCases =>
        new List<object[]>
        {
            new object[] { "Title", null },
            new object[] { "title", null },
            new object[] { "Title", SortOrder.Asc },
            new object[] { "title", SortOrder.Asc }
        };
    public static IEnumerable<object[]> StatusSortingTestCases =>
        new List<object[]>
        {
            new object[] { "Status", null },
            new object[] { "status", null },
            new object[] { "Status", SortOrder.Asc },
            new object[] { "status", SortOrder.Asc }
        };
}
