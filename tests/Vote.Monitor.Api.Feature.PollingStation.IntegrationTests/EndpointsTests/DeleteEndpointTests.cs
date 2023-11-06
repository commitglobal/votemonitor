
namespace Vote.Monitor.Api.Feature.PollingStation.IntegrationTests.EndpointsTests;

public class DeleteEndpointTests : IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public DeleteEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_DeletePollingStation_WhenSuchExists()
    {
        // Arrange
        var newPollingStation = Fixture.Fake.CreateRequest();
        var (createResponse, createResult) = await Fixture.PlatformAdmin.POSTAsync<Create.Endpoint, Create.Request, PollingStationModel>(newPollingStation);

        createResponse.IsSuccessStatusCode.Should().BeTrue();

        var request = new Delete.Request
        {
            Id = createResult.Id
        };

        // Act
        var deleteResponse = await Fixture.PlatformAdmin.DELETEAsync<Delete.Endpoint, Delete.Request>(request);
        
        // Assert
        deleteResponse.IsSuccessStatusCode.Should().BeTrue();

        var getResponse = await Fixture.PlatformAdmin.GETAsync<Get.Endpoint, Get.Request>(new()
        {
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
        var (createResponse, createResult) = await Fixture.PlatformAdmin.POSTAsync<Create.Endpoint, Create.Request, PollingStationModel>(newPollingStation);

        createResponse.IsSuccessStatusCode.Should().BeTrue();

        var request = new Delete.Request
        {
            Id = Guid.NewGuid()
        };

        // Act
        var deleteResponse = await Fixture.PlatformAdmin.DELETEAsync<Delete.Endpoint, Delete.Request>(request);

        // Assert
        deleteResponse.IsSuccessStatusCode.Should().BeFalse();
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var (getResponse, pollingStation) = await Fixture.PlatformAdmin.GETAsync<Get.Endpoint, Get.Request, PollingStationModel>(new()
        {
            Id = createResult.Id
        });

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        pollingStation.Should().BeEquivalentTo(newPollingStation);
        pollingStation.Id.Should().Be(createResult.Id);
    }
}
