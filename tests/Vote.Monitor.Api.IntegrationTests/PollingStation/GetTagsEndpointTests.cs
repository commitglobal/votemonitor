using GetTagsEndpoint = Vote.Monitor.Api.Feature.PollingStation.GetTags.Endpoint;

namespace Vote.Monitor.Api.IntegrationTests.PollingStation;

public class GetTagsEndpointTests : IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public GetTagsEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_Return_AllAvailableTags()
    {
        // Arrange
        await Fixture.PlatformAdmin.ImportPollingStations();
        // Act
        var (getTagsResult, tags) = await Fixture.PlatformAdmin.GETAsync<GetTagsEndpoint, List<string>>();

        // Assert
        getTagsResult.IsSuccessStatusCode.Should().BeTrue();
        tags
            .Should()
            .HaveCount(7)
            .And.BeEquivalentTo("Country", "Gmina", "Powiat", "Województwo", "Judet", "UAT", "Number");
    }
}
