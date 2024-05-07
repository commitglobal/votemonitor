using Vote.Monitor.Core.Constants;
using GetEndpoint = Vote.Monitor.Api.Feature.UserPreferences.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.UserPreferences.Get.Request;
using UpdateEndpoint = Vote.Monitor.Api.Feature.UserPreferences.Update.Endpoint;
using UpdateRequest = Vote.Monitor.Api.Feature.UserPreferences.Update.Request;
using UserPreferencesModel = Vote.Monitor.Api.Feature.UserPreferences.UserPreferencesModel;

namespace Vote.Monitor.Api.IntegrationTests.UserPreferences;

[Collection("IntegrationTests")]
public class UpdateEndpointTests : IClassFixture<HttpServerFixture<NoopDataSeeder>>
{
    public HttpServerFixture<NoopDataSeeder> Fixture { get; }

    public UpdateEndpointTests(HttpServerFixture<NoopDataSeeder> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task UpdateUserPreferences_ReturnsSuccessStatusCode_WhenTheUpdateRequestIsValid()
    {
        // Arrange
        string languageCode = LanguagesList.RO.Iso1;
        var updateRequest = new UpdateRequest
        {
            LanguageCode = languageCode
        };

        UserPreferencesModel userPreferences = new() { LanguageCode = languageCode };

        // Act
        var updateResponse = await Fixture.PlatformAdmin.POSTAsync<UpdateEndpoint, UpdateRequest>(updateRequest);

        // Assert
        updateResponse.EnsureSuccessStatusCode();
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResponse = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, UserPreferencesModel>(new GetRequest { Id = updateRequest.Id });
        getResponse.Response.EnsureSuccessStatusCode();
        getResponse.Result.Should().BeEquivalentTo(userPreferences);
    }

    [Fact]
    public async Task UpdateUserPreferences_ReturnsNotFound_WhenLanguageIdIsInvalid()
    {
        // Arrange
        string languageCode = "UNKNOWN";
        var updateRequest = new UpdateRequest
        {
            LanguageCode = languageCode
        };

        // Act
        var updateResponse = await Fixture.PlatformAdmin.POSTAsync<UpdateEndpoint, UpdateRequest, UserPreferencesModel>(updateRequest);

        // Assert
        updateResponse.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUserPreferences_ReturnsBadRequest_WhenLanguageIdIsEmpty()
    {
        // Arrange
        var updateRequest = new UpdateRequest();

        // Act
        var (updateResponse, errors) = await Fixture.PlatformAdmin.POSTAsync<UpdateEndpoint, UpdateRequest, FEProblemDetails>(updateRequest);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors.Errors.Count().Should().Be(1);
        errors.Errors.First().Reason.Should().Be("'Language Code' must not be empty.");
    }
}
