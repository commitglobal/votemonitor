using GetEndpoint = Vote.Monitor.Api.Feature.UserPreferences.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.UserPreferences.Get.Request;
using UppdateEndpoint = Vote.Monitor.Api.Feature.UserPreferences.Update.Endpoint;
using UpdateRequest = Vote.Monitor.Api.Feature.UserPreferences.Update.Request;
using UserPreferencesModel = Vote.Monitor.Api.Feature.UserPreferences.UserPreferencesModel;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Vote.Monitor.Api.IntegrationTests.UserPreferences;
public class UpdateEndpointTests : IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public UpdateEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

  

    [Fact]
    public async Task UpdateUserPreferences_ReturnsSuccessStatusCode_WhenTheUpdateRequestIsValid()
    {
        // Arrange
        Guid languageId = new Guid("094b3769-68b1-6211-ba2d-6bba92d6a167");
        var updateRequest = new UpdateRequest
        {
            LanguageId = languageId
        };

        UserPreferencesModel userPreferences = new (){LanguageId = languageId};

        // Act
        var updateResponse = await Fixture.PlatformAdmin.POSTAsync<UppdateEndpoint, UpdateRequest, NoContent>(updateRequest);

        // Assert
        updateResponse.Response.EnsureSuccessStatusCode();
        updateResponse.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResponse = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, UserPreferencesModel>(new GetRequest { Id = updateRequest.Id });
        getResponse.Response.EnsureSuccessStatusCode();
        getResponse.Result.Should().BeEquivalentTo(userPreferences);
    }

    [Fact]
    public async Task UpdateUserPreferences_ReturnsNotFound_WhenLanguageIdIsInvalid()
    {
        // Arrange
        Guid languageId = Guid.NewGuid();
        var updateRequest = new UpdateRequest
        {
            LanguageId = languageId
        };

        // Act
        var updateResponse = await Fixture.PlatformAdmin.POSTAsync<UppdateEndpoint, UpdateRequest, UserPreferencesModel>(updateRequest);

        // Assert
        updateResponse.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUserPreferences_ReturnsBadRequest_WhenLanguageIdIsEmpty()
    {
        // Arrange
        var updateRequest = new UpdateRequest();

        // Act
        var (updateResponse, errors) = await Fixture.PlatformAdmin.POSTAsync<UppdateEndpoint, UpdateRequest, FEProblemDetails>(updateRequest);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors.Errors.Count().Should().Be(1);
        errors.Errors.Any(e => string.Equals(e.Reason, "'Language Id' must not be empty.")).Should().BeTrue();
    }
}
