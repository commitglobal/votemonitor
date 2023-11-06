using Microsoft.AspNetCore.Http;

namespace Vote.Monitor.Feature.PollingStation.IntegrationTests.EndpointsTests;

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
        using var testDataStream = File.OpenRead("test-data.csv");
        var request = new Import.Request
        {
            File = new FormFile(testDataStream, 0, testDataStream.Length, "test-data.csv", "test-data.csv"),
        };
        _ = await Fixture.PlatformAdmin.POSTAsync<Import.Endpoint, Import.Request, Import.Response>(request, true);
       
        // Act
        var (getTagsResult, tags) = await Fixture.PlatformAdmin.GETAsync<GetTags.Endpoint, List<string>>();

        // Assert
        getTagsResult.IsSuccessStatusCode.Should().BeTrue();
        tags
            .Should()
            .HaveCount(7)
            .And.BeEquivalentTo("Country", "Gmina", "Powiat", "Województwo", "Judet", "UAT", "Number");
    }
}
