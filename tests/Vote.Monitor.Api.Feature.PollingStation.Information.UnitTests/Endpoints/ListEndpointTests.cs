namespace Vote.Monitor.Api.Feature.PollingStation.Information.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IReadRepository<PollingStationInformation> _repository;
    private readonly List.Endpoint _endpoint;

    public ListEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<PollingStationInformation>>();
        _endpoint = Factory.Create<List.Endpoint>(_repository);
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
            .ListAsync(Arg.Any<GetPollingStationInformationForNgoSpecification>())
            .Returns(pollingStationInformations);

        // Act
        var request = new List.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should()
            .BeOfType<Ok<List.Response>>()
            .Which.Value!.Informations
            .Should().BeEquivalentTo(pollingStationInformations);
    }
}
