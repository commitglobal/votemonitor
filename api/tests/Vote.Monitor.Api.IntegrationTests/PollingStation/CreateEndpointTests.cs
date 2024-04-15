using Vote.Monitor.Api.Feature.PollingStation;
using CreateEndpoint = Vote.Monitor.Api.Feature.PollingStation.Create.Endpoint;
using CreateRequest = Vote.Monitor.Api.Feature.PollingStation.Create.Request;

using GetEndpoint = Vote.Monitor.Api.Feature.PollingStation.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.PollingStation.Get.Request;

namespace Vote.Monitor.Api.IntegrationTests.PollingStation;

public class CreateEndpointTests : IClassFixture<HttpServerFixture<NoopDataSeeder>>
{
    public HttpServerFixture<NoopDataSeeder> Fixture { get; }

    public CreateEndpointTests(HttpServerFixture<NoopDataSeeder> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_CreatePollingStation_WhenValidRequestData()
    {
        // Arrange
        var newPollingStation = Fixture.Fake.CreateRequest(Fixture.ElectionRound);

        // Act
        var (createResponse, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, PollingStationModel>(newPollingStation);

        // Assert
        createResponse.IsSuccessStatusCode.Should().BeTrue();
        createResult.Id.Should().NotBeEmpty();

        var request = new GetRequest
        {
            ElectionRoundId = Fixture.ElectionRound.Id,
            Id = createResult.Id
        };
        var (getResponse, pollingStation) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, PollingStationModel>(request);

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        pollingStation.Should().BeEquivalentTo(newPollingStation, opt => opt.Excluding(x => x.ElectionRoundId));
        pollingStation.Id.Should().Be(createResult.Id);
    }

    [Fact]
    public async Task Should_NotCreatePollingStation_WhenInvalidRequestData()
    {
        // Arrange
        var newPollingStation = new CreateRequest
        {
            ElectionRoundId = Guid.Empty,
            Address = "",
            DisplayOrder = -1,
            Tags = null
        };

        // Act
        var (createResponse, errorResponse) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, FEProblemDetails>(newPollingStation);

        // Assert
        createResponse.IsSuccessStatusCode.Should().BeFalse();
        errorResponse.Errors.Count().Should().Be(5);
    }
}
