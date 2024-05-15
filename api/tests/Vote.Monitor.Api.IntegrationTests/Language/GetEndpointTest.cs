using Vote.Monitor.Api.Feature.Language;
using Vote.Monitor.Api.Feature.Language.Get;

namespace Vote.Monitor.Api.IntegrationTests.Language;

[Collection("IntegrationTests")]
public class GetEndpointTest : IClassFixture<HttpServerFixture<NoopDataSeeder>>
{
    public HttpServerFixture<NoopDataSeeder> Fixture { get; }

    public GetEndpointTest(HttpServerFixture<NoopDataSeeder> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Theory]
    [InlineData("094b3769-68b1-6211-ba2d-6bba92d6a167", "EN", "English")]
    [InlineData("5f002f07-f2c3-9fa4-2e29-225d116c10a3", "SW", "Swahili")]

    public async Task Should_ReturnCorrectLanguage(Guid id, string code, string languageName)
    {
        // Arrange
        var request = new Request
        {
            Id = id
        };

        // Act
        var (response, result) = await Fixture.PlatformAdmin.GETAsync<Endpoint, Request, LanguageModel>(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Code.Should().Be(code);
        result.Name.Should().Be(languageName);
    }

    [Theory]
    [InlineData("899c2a9f-f35d-5a49-a6cd-f92531112266")]
    public async Task Should_ReturnNotFound_WhenInvalidLanguageGuid(Guid id)
    {
        // Arrange
        var request = new Request
        {
            Id = id
        };

        // Act
        var (response, _) = await Fixture.PlatformAdmin.GETAsync<Endpoint, Request, LanguageModel>(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
