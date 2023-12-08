using Vote.Monitor.Api.Feature.Language;
using Vote.Monitor.Api.Feature.Language.Get;

namespace Vote.Monitor.Api.IntegrationTests.Language;

public class GetEndpointTest : IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public GetEndpointTest(HttpServerFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }


    [Theory]
    [InlineData("2b03fe82-33d9-5d7b-fb99-b28717cf8651", "eng", "English")]
    [InlineData("68c51165-bb8b-7987-8128-695a283411a2", "fas", "Persian")]

    public async Task Shoul_ReturnCorrectLanguage(Guid id, string code, string languageName)
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
        result.Iso3.Should().Be(code);
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
