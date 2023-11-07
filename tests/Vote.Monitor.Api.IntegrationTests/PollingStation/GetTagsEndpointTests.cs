using Microsoft.AspNetCore.Http;
using ImportEndpoint = Vote.Monitor.Api.Feature.PollingStation.Import.Endpoint;
using ImportRequest = Vote.Monitor.Api.Feature.PollingStation.Import.Request;
using ImportResponse = Vote.Monitor.Api.Feature.PollingStation.Import.Request;

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
        using var testDataStream = File.OpenRead("test-data.csv");
        var request = new ImportRequest
        {
            File = new FormFile(testDataStream, 0, testDataStream.Length, "test-data.csv", "test-data.csv"),
        };
        _ = await Fixture.PlatformAdmin.POSTAsync<ImportEndpoint, ImportRequest, ImportResponse> (request, true);
       
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
