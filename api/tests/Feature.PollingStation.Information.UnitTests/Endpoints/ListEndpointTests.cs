using Feature.PollingStation.Information.List;
using Feature.PollingStation.Information.Specifications;

namespace Feature.PollingStation.Information.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IReadRepository<PollingStationInformation> _repository;
    private readonly Endpoint _endpoint;

    public ListEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<PollingStationInformation>>();
        _endpoint = Factory.Create<Endpoint>(_repository);
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
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should()
            .BeOfType<Ok<Response>>()
            .Which.Value!.Informations
            .Should().BeEquivalentTo(pollingStationInformations);
    }
}
