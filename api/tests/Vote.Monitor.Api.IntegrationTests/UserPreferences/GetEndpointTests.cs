using Vote.Monitor.Api.Feature.UserPreferences;
using GetEndpoint = Vote.Monitor.Api.Feature.UserPreferences.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.UserPreferences.Get.Request;

namespace Vote.Monitor.Api.IntegrationTests.UserPreferences;

[Collection("IntegrationTests")]
public class GetEndpointTests : IClassFixture<HttpServerFixture<NoopDataSeeder>>
{
    public HttpServerFixture<NoopDataSeeder> Fixture { get; }

    public GetEndpointTests(HttpServerFixture<NoopDataSeeder> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_ReturnsUserPreferences_WhenUserExists()
    {
        // Arrange
        GetRequest getRequest = new GetRequest();

        // Act
        var (getResponse, userPreferences) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, UserPreferencesModel>(getRequest);
        
        // Assert
        getResponse.IsSuccessStatusCode.Should().BeTrue();
        userPreferences.Should().NotBeNull();
    }
}
