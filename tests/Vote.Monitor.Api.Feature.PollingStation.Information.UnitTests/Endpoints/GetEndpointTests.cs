namespace Vote.Monitor.Api.Feature.PollingStation.Information.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IReadRepository<PollingStationInformation> _repository;
    private readonly Get.Endpoint _endpoint;

    public GetEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<PollingStationInformation>>();
        _endpoint = Factory.Create<Get.Endpoint>(_repository);
    }

    [Fact]
    public async Task Should_GetPollingStationInformation_And_ReturnNoContent_WhenExists()
    {
        // Arrange
        var pollingStationInformation = new PollingStationInformationFaker().Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationSpecification>())
            .Returns(pollingStationInformation);

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert

        result
            .Should().BeOfType<Results<Ok<PollingStationInformationModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<PollingStationInformationModel>>()
            .Which.Value.Should().BeEquivalentTo(pollingStationInformation, opt => opt.ExcludingMissingMembers());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenPollingStationInformationNotFound()
    {
        // Arrange
        _repository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationSpecification>())
            .ReturnsNull();

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStationId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert

        result
            .Should().BeOfType<Results<Ok<PollingStationInformationModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
