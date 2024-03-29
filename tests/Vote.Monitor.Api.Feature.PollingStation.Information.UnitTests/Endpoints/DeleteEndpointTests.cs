namespace Vote.Monitor.Api.Feature.PollingStation.Information.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    private readonly IRepository<PollingStationInformation> _repository;
    private readonly Delete.Endpoint _endpoint;

    public DeleteEndpointTests()
    {
        _repository = Substitute.For<IRepository<PollingStationInformation>>();
        _endpoint = Factory.Create<Delete.Endpoint>(_repository);
    }

    [Fact]
    public async Task Should_DeletePollingStationInformation_And_ReturnNoContent_WhenExists()
    {
        // Arrange
        var pollingStationInformation = new PollingStationInformationFaker().Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationSpecification>())
            .Returns(pollingStationInformation);

        // Act
        var request = new Delete.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository.Received(1).DeleteAsync(pollingStationInformation);

        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenPollingStationInformationNotFound()
    {
        // Arrange
        _repository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationSpecification>())
            .ReturnsNull();

        // Act
        var request = new Delete.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository.DidNotReceiveWithAnyArgs().DeleteAsync(Arg.Any<PollingStationInformation>());

        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
