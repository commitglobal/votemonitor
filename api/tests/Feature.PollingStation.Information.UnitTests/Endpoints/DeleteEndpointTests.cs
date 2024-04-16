using Feature.PollingStation.Information.Delete;
using Feature.PollingStation.Information.Specifications;

namespace Feature.PollingStation.Information.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    private readonly IRepository<PollingStationInformation> _repository;
    private readonly Endpoint _endpoint;

    public DeleteEndpointTests()
    {
        _repository = Substitute.For<IRepository<PollingStationInformation>>();
        _endpoint = Factory.Create<Endpoint>(_repository);
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
        var request = new Request
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
        var request = new Request
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
