namespace Vote.Monitor.Api.Feature.ElectionRound.UnitTests.Endpoints;

public class GetEndpointTests
{
    [Fact]
    public async Task Should_ReturnElectionRound_WhenExists()
    {
        // Arrange
        var electionRoundModel = new ElectionRoundModel
        {
            Id = Guid.NewGuid(),
            Title = "A title",
            StartDate = new DateOnly(2024, 01, 02),
            EnglishTitle = "An english title",
            Status = ElectionRoundStatus.NotStarted,
            Country = CountriesList.MD.Name,
            CountryId = CountriesList.MD.Id,
            CreatedOn = DateTime.UtcNow.AddHours(-30),
            LastModifiedOn = DateTime.UtcNow.AddHours(-15)
        };

        var repository = Substitute.For<IReadRepository<ElectionRoundAggregate>>();
        repository
            .SingleOrDefaultAsync(Arg.Any<GetElectionRoundByIdSpecification>())
            .Returns(electionRoundModel);

        var endpoint = Factory.Create<Get.Endpoint>(repository);

        // Act
        var request = new Get.Request { Id = electionRoundModel.Id };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<ElectionRoundModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<ElectionRoundModel>>()
            .Which.Value.Should().BeEquivalentTo(electionRoundModel);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenElectionRoundNotFound()
    {
        // Arrange
        var repository = Substitute.For<IReadRepository<ElectionRoundAggregate>>();
        var endpoint = Factory.Create<Get.Endpoint>(repository);

        // Act
        var request = new Get.Request { Id = Guid.NewGuid() };
        var result = await endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<ElectionRoundModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
