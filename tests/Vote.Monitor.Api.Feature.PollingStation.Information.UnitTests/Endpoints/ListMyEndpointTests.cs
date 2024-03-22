namespace Vote.Monitor.Api.Feature.PollingStation.Information.UnitTests.Endpoints;

public class ListMyEndpointTests
{
    private readonly IReadRepository<PollingStationInformation> _repository;
    private readonly ListMy.Endpoint _endpoint;

    public ListMyEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<PollingStationInformation>>();
        _endpoint = Factory.Create<ListMy.Endpoint>(_repository);
    }

    [Fact]
    public async Task Should_GetPollingStationInformation_ForNgo()
    {
        // Arrange
        var pollingStationInformations = new PollingStationInformationFaker()
            .Generate(10)
            .Select(PollingStationInformationModel.FromEntity)
            .ToList();

        _repository
            .ListAsync(Arg.Any<GetPollingStationInformationForObserverSpecification>())
            .Returns(pollingStationInformations);

        // Act
        var request = new ListMy.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should()
            .BeOfType<Ok<ListMy.Response>>()
            .Which.Value!.Informations
            .Should().BeEquivalentTo(pollingStationInformations);
    }
}
