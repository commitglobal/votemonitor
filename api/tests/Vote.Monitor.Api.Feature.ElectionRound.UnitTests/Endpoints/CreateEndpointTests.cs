using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Api.Feature.ElectionRound.UnitTests.Endpoints;

public class CreateEndpointTests
{
    [Fact]
    public async Task ShouldReturnOkWithElectionRoundModel_WhenNoConflict()
    {
        // Arrange
        var electionRoundTitle = "some local title";
        var englishTitle = "some english title";
        var startDate = new DateOnly(2024, 01, 02);

        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        var psiFormRepository = Substitute.For<IRepository<PollingStationInformationForm>>();

        repository
            .AnyAsync(Arg.Any<GetActiveElectionRoundSpecification>())
            .Returns(false);

        var endpoint = Factory.Create<Create.Endpoint>(repository, psiFormRepository);

        // Act
        var request = new Create.Request
        {
            Title = electionRoundTitle,
            EnglishTitle = englishTitle,
            StartDate = startDate,
            CountryId = CountriesList.MD.Id
        };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        await repository
            .Received(1)
            .AddAsync(Arg.Is<ElectionRoundAggregate>(x => x.Title == electionRoundTitle
                                                          && x.EnglishTitle == englishTitle
                                                          && x.StartDate == startDate));

        var model = result.Result.As<Ok<ElectionRoundModel>>();
        model.Value.Title.Should().Be(electionRoundTitle);
        model.Value.EnglishTitle.Should().Be(englishTitle);
        model.Value.StartDate.Should().Be(startDate);
        model.Value.Status.Should().Be(ElectionRoundStatus.NotStarted);
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenElectionRoundWithSameTitleExists()
    {
        // Arrange
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        var psiFormRepository = Substitute.For<IRepository<PollingStationInformationForm>>();

        repository
            .AnyAsync(Arg.Any<GetActiveElectionRoundSpecification>())
            .Returns(true);

        var endpoint = Factory.Create<Create.Endpoint>(repository, psiFormRepository);

        // Act
        var request = new Create.Request
            { Title = "a title", EnglishTitle = "an english title", StartDate = DateOnly.MinValue };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<ElectionRoundModel>, Conflict<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<Conflict<ProblemDetails>>();
    }


    [Fact]
    public async Task ShouldCreate_PSIForm()
    {
        // Arrange
        var repository = Substitute.For<IRepository<ElectionRoundAggregate>>();
        var psiFormRepository = Substitute.For<IRepository<PollingStationInformationForm>>();

        var endpoint = Factory.Create<Create.Endpoint>(repository, psiFormRepository);

        // Act
        var request = new Create.Request
        {
            Title = "a title",
            EnglishTitle = "an english title",
            StartDate = DateOnly.MinValue,
            CountryId = CountriesList.MD.Id
        };
        await endpoint.ExecuteAsync(request, default);

        // Assert
        await psiFormRepository.Received(1)
            .AddAsync(Arg.Is<PollingStationInformationForm>(f => f.Questions.Count == 1));
    }
}