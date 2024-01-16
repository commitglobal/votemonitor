using Vote.Monitor.Api.Feature.Language;

namespace Vote.Monitor.Api.IntegrationTests.Language;

public class ListEndpointTests:IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public ListEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_ReturnAllLanguages()
    {
        // Arrange &c Act
        var (response, result) = await Fixture.PlatformAdmin.GETAsync<Api.Feature.Language.List.Endpoint, List<LanguageModel>>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Count.Should().Be(183);
    }

    [Fact]
    public async Task Should_Check3RandomLanguages()
    {
        // Arrange
        List<Tuple<string, string>> testLanguages = new List<Tuple<string, string>>
        {
            new("EN", "English"), 
            new("FA", "Persian"),
        };

        // Act
        var (response, result) = await Fixture.PlatformAdmin.GETAsync<Vote.Monitor.Api.Feature.Language.List.Endpoint, List<LanguageModel>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        foreach(var language in testLanguages)
        {
            result.Should().Contain(x => x.Code == language.Item1 && x.Name == language.Item2);
        }
    }
}
