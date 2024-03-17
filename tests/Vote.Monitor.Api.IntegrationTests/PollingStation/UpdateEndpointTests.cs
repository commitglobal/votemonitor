using Vote.Monitor.Api.Feature.PollingStation;
using CreateEndpoint = Vote.Monitor.Api.Feature.PollingStation.Create.Endpoint;
using CreateRequest = Vote.Monitor.Api.Feature.PollingStation.Create.Request;

using GetEndpoint = Vote.Monitor.Api.Feature.PollingStation.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.PollingStation.Get.Request;

using UpdateEndpoint = Vote.Monitor.Api.Feature.PollingStation.Update.Endpoint;
using UpdateRequest = Vote.Monitor.Api.Feature.PollingStation.Update.Request;

namespace Vote.Monitor.Api.IntegrationTests.PollingStation;

public class UpdateEndpointTests : IClassFixture<HttpServerFixture<NoopDataSeeder>>
{
    public HttpServerFixture<NoopDataSeeder> Fixture { get; }

    public UpdateEndpointTests(HttpServerFixture<NoopDataSeeder> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_UpdatePollingStation_WhenSuchExists()
    {
        // Arrange
        var createRequest = Fixture.Fake.CreateRequest(Fixture.ElectionRound);
        var (createResponse, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, PollingStationModel>(createRequest);
        createResponse.IsSuccessStatusCode.Should().BeTrue();

        // Act
        var updateRequest = Fixture.Fake.UpdateRequest(createResult.Id, Fixture.ElectionRound);
        var updateResponse = await Fixture.PlatformAdmin.PUTAsync<UpdateEndpoint, UpdateRequest>(updateRequest);

        // Assert
        updateResponse.IsSuccessStatusCode.Should().BeTrue();

        var (getResponse, pollingStation) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, PollingStationModel>(new()
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = createResult.Id
        });

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        pollingStation.Should().BeEquivalentTo(updateRequest);
    }

    [Fact]
    public async Task Should_ReturnNotFound_WhenNoSuchExists()
    {
        // Arrange
        var newPollingStation = Fixture.Fake.CreateRequest(Fixture.ElectionRound);
        var (createResponse, createResult) = await Fixture.PlatformAdmin.POSTAsync<Feature.PollingStation.Create.Endpoint, Feature.PollingStation.Create.Request, PollingStationModel>(newPollingStation);

        createResponse.IsSuccessStatusCode.Should().BeTrue();

        // Act
        var updateRequest = Fixture.Fake.UpdateRequest(Guid.NewGuid(), Fixture.ElectionRound);
        var updateResponse = await Fixture.PlatformAdmin.PUTAsync<UpdateEndpoint, UpdateRequest>(updateRequest);

        // Assert
        updateResponse.IsSuccessStatusCode.Should().BeFalse();
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var (getResponse, pollingStation) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, PollingStationModel>(new()
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = createResult.Id
        });

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        pollingStation.Should().BeEquivalentTo(newPollingStation);
        pollingStation.Id.Should().Be(createResult.Id);
    }

    [Fact]
    public async Task Should_NotUpdate_WhenInvalidData()
    {
        // Arrange
        var newPollingStation = Fixture.Fake.CreateRequest(Fixture.ElectionRound);
        var (createResponse, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, PollingStationModel>(newPollingStation);

        createResponse.IsSuccessStatusCode.Should().BeTrue();

        var updateRequest = new UpdateRequest
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = createResult.Id,
            Address = "",
            DisplayOrder = -1,
            Tags = null
        };

        // Act
        var (updateResponse, errorResponse) = await Fixture.PlatformAdmin.PUTAsync<UpdateEndpoint, UpdateRequest, FEProblemDetails>(updateRequest);

        // Assert
        updateResponse.IsSuccessStatusCode.Should().BeFalse();
        errorResponse.Errors.Count().Should().Be(3);

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
