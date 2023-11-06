namespace Vote.Monitor.Api.Feature.PollingStation.IntegrationTests.EndpointsTests;

public class CreateEndpointTests : IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public CreateEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_CreatePollingStation_WhenValidRequestData()
    {
        // Arrange
        var newPollingStation = Fixture.Fake.CreateRequest();

        // Act
        var (createResponse, createResult) = await Fixture.PlatformAdmin.POSTAsync<Create.Endpoint, Create.Request, PollingStationModel>(newPollingStation);

        // Assert
        createResponse.IsSuccessStatusCode.Should().BeTrue();
        createResult.Id.Should().NotBeEmpty();

        var request = new Api.Feature.PollingStation.Get.Request
        {
            Id = createResult.Id
        };
        var (getResponse, pollingStation) = await Fixture.PlatformAdmin.GETAsync<Get.Endpoint, Get.Request, PollingStationModel>(request);

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        pollingStation.Should().BeEquivalentTo(newPollingStation);
        pollingStation.Id.Should().Be(createResult.Id);
    }

    [Fact]
    public async Task Should_NotCreatePollingStation_WhenInvalidRequestData()
    {
        // Arrange
        var newPollingStation = new Create.Request
        {
            Address = "",
            DisplayOrder = -1,
            Tags = null
        };

        // Act
        var (createResponse, errorResponse) = await Fixture.PlatformAdmin.POSTAsync<Create.Endpoint, Create.Request, ErrorResponse>(newPollingStation);

        // Assert
        createResponse.IsSuccessStatusCode.Should().BeFalse();
        errorResponse.Errors.Count.Should().Be(3);
    }
}
