namespace Vote.Monitor.Api.Feature.PollingStation.IntegrationTests.EndpointsTests;

public class UpdateEndpointTests : IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public UpdateEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_UpdatePollingStation_WhenSuchExists()
    {
        // Arrange
        var createRequest = Fixture.Fake.CreateRequest();
        var (createResponse, createResult) = await Fixture.PlatformAdmin.POSTAsync<Create.Endpoint, Create.Request, PollingStationModel>(createRequest);
        createResponse.IsSuccessStatusCode.Should().BeTrue();

        // Act
        var updateRequest = Fixture.Fake.UpdateRequest(createResult.Id);
        var updateResponse = await Fixture.PlatformAdmin.PUTAsync<Update.Endpoint, Update.Request>(updateRequest);
        
        // Assert
        updateResponse.IsSuccessStatusCode.Should().BeTrue();

        var (getResponse, pollingStation) = await Fixture.PlatformAdmin.GETAsync<Get.Endpoint, Get.Request, PollingStationModel>(new()
        {
            Id = createResult.Id
        });

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        pollingStation.Should().BeEquivalentTo(updateRequest);
    }

    [Fact]
    public async Task Should_ReturnNotFound_WhenNoSuchExists()
    {
        // Arrange
        var newPollingStation = Fixture.Fake.CreateRequest();
        var (createResponse, createResult) = await Fixture.PlatformAdmin.POSTAsync<Create.Endpoint, Create.Request, PollingStationModel>(newPollingStation);

        createResponse.IsSuccessStatusCode.Should().BeTrue();

        // Act
        var updateRequest = Fixture.Fake.UpdateRequest(Guid.NewGuid());
        var updateResponse = await Fixture.PlatformAdmin.PUTAsync<Update.Endpoint, Update.Request>(updateRequest);

        // Assert
        updateResponse.IsSuccessStatusCode.Should().BeFalse();
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var (getResponse, pollingStation) = await Fixture.PlatformAdmin.GETAsync<Get.Endpoint, Get.Request, PollingStationModel>(new()
        {
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
        var newPollingStation = Fixture.Fake.CreateRequest();
        var (createResponse, createResult) = await Fixture.PlatformAdmin.POSTAsync<Create.Endpoint, Create.Request, PollingStationModel>(newPollingStation);

        createResponse.IsSuccessStatusCode.Should().BeTrue();

        var updateRequest = new Update.Request
        {
            Id = createResult.Id,
            Address = "",
            DisplayOrder = -1,
            Tags = null
        };

        // Act
        var (updateResponse, errorResponse) = await Fixture.PlatformAdmin.PUTAsync<Update.Endpoint, Update.Request, ErrorResponse>(updateRequest);

        // Assert
        updateResponse.IsSuccessStatusCode.Should().BeFalse();
        errorResponse.Errors.Count.Should().Be(3);

        var (getResponse, pollingStation) = await Fixture.PlatformAdmin.GETAsync<Get.Endpoint, Get.Request, PollingStationModel>(new()
        {
            Id = createResult.Id
        });

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        pollingStation.Should().BeEquivalentTo(newPollingStation);
        pollingStation.Id.Should().Be(createResult.Id);
    }
}
