using Vote.Monitor.Api.Feature.PollingStation;
using CreateEndpoint = Vote.Monitor.Api.Feature.PollingStation.Create.Endpoint;
using CreateRequest = Vote.Monitor.Api.Feature.PollingStation.Create.Request;

using GetEndpoint = Vote.Monitor.Api.Feature.PollingStation.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.PollingStation.Get.Request;

using DeleteEndpoint = Vote.Monitor.Api.Feature.PollingStation.Delete.Endpoint;
using DeleteRequest = Vote.Monitor.Api.Feature.PollingStation.Delete.Request;

namespace Vote.Monitor.Api.IntegrationTests.PollingStation;

public class DeleteEndpointTests : IClassFixture<HttpServerFixture<NoopDataSeeder>>
{
    public HttpServerFixture<NoopDataSeeder> Fixture { get; }

    public DeleteEndpointTests(HttpServerFixture<NoopDataSeeder> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_DeletePollingStation_WhenSuchExists()
    {
        // Arrange
        var newPollingStation = Fixture.Fake.CreateRequest();
        var (createResponse, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, PollingStationModel>(newPollingStation);

        createResponse.IsSuccessStatusCode.Should().BeTrue();

        var request = new DeleteRequest
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = createResult.Id
        };

        // Act
        var deleteResponse = await Fixture.PlatformAdmin.DELETEAsync<DeleteEndpoint, DeleteRequest>(request);

        // Assert
        deleteResponse.IsSuccessStatusCode.Should().BeTrue();

        var getResponse = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest>(new()
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = createResult.Id
        });

        getResponse.IsSuccessStatusCode.Should().BeFalse();
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_ReturnNotFound_WhenNoSuchExists()
    {
        // Arrange
        var newPollingStation = Fixture.Fake.CreateRequest();
        var (createResponse, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, PollingStationModel>(newPollingStation);

        createResponse.IsSuccessStatusCode.Should().BeTrue();

        var request = new DeleteRequest
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };

        // Act
        var deleteResponse = await Fixture.PlatformAdmin.DELETEAsync<DeleteEndpoint, DeleteRequest>(request);

        // Assert
        deleteResponse.IsSuccessStatusCode.Should().BeFalse();
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var (getResponse, pollingStation) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, PollingStationModel>(new()
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = createResult.Id
        });

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        pollingStation.Should().BeEquivalentTo(newPollingStation);
        pollingStation.Id.Should().Be(createResult.Id);
    }
}
