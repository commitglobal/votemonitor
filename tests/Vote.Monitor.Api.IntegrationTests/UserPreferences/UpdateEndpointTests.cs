using GetEndpoint = Vote.Monitor.Api.Feature.UserPreferences.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.UserPreferences.Get.Request;
using UppdateEndpoint = Vote.Monitor.Api.Feature.UserPreferences.Update.Endpoint;
using UpdateRequest = Vote.Monitor.Api.Feature.UserPreferences.Update.Request;
using UserPreferencesModel = Vote.Monitor.Api.Feature.UserPreferences.UserPreferencesModel;
using Vote.Monitor.Domain.Constants;
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
    public async Task UpdateUserPreferences_ReturnsSuccessStatusCode_WhenTheUpdateRequestIsValidAndContaingOnlyLanguageIsoName()
    {
        // Arrange
        string languageIso = "EN";
        var updateRequest = new UpdateRequest
        {
            LanguageIso = languageIso,
            Preferences = null
        };

        Dictionary<string, string> userPreferences = new Dictionary<string, string>
        {
            { "LanguageIso", languageIso },
            { "LanguageId", LanguagesList.GetByIso(languageIso.ToUpperInvariant())!.Id.ToString() }
        };

        // Act
        var updateResponse = await Fixture.PlatformAdmin.POSTAsync<UppdateEndpoint, UpdateRequest, UserPreferencesModel>(updateRequest);

        // Assert
        updateResponse.Response.EnsureSuccessStatusCode();
        updateResponse.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResponse = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, Dictionary<string, string>>(new GetRequest { Id = updateRequest.Id });
        getResponse.Response.EnsureSuccessStatusCode();
        getResponse.Result.Should().BeEquivalentTo(userPreferences);
    }

    [Fact]
    public async Task UpdateUserPreferences_ReturnsSuccessStatusCode_WhenTheUpdateRequestIsValidAndContaingOnlyLanguageIDValid()
    {
        // Arrange
        Guid languageId = new Guid("094b3769-68b1-6211-ba2d-6bba92d6a167");
        var updateRequest = new UpdateRequest
        {
            LanguageId = languageId,
            Preferences = null
        };

        Dictionary<string, string> userPreferences = new Dictionary<string, string>
        {
            { "LanguageIso", "EN" },
            { "LanguageId", languageId.ToString()}
        };

        // Act
        var updateResponse = await Fixture.PlatformAdmin.POSTAsync<UppdateEndpoint, UpdateRequest, UserPreferencesModel>(updateRequest);

        // Assert
        updateResponse.Response.EnsureSuccessStatusCode();
        updateResponse.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResponse = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, Dictionary<string, string>>(new GetRequest { Id = updateRequest.Id });
        getResponse.Response.EnsureSuccessStatusCode();
        getResponse.Result.Should().BeEquivalentTo(userPreferences);
    }

    [Fact]
    public async Task UpdateUserPreferences_ReturnsSuccessStatusCode_WhenTheUpdateRequestIsValidAndContaingLanguageIDandLanguageIso()
    {
        // Arrange
        string languageIso = "EN";
        Guid languageId = new Guid("094b3769-68b1-6211-ba2d-6bba92d6a167");
        var updateRequest = new UpdateRequest
        {
            LanguageId = languageId,
            LanguageIso = languageIso,
            Preferences = null
        };

        Dictionary<string, string> userPreferences = new Dictionary<string, string>
        {
            { "LanguageIso",languageIso },
            { "LanguageId", languageId.ToString()}
        };

        // Act
        var updateResponse = await Fixture.PlatformAdmin.POSTAsync<UppdateEndpoint, UpdateRequest, UserPreferencesModel>(updateRequest);

        // Assert
        updateResponse.Response.EnsureSuccessStatusCode();
        updateResponse.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResponse = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, Dictionary<string, string>>(new GetRequest { Id = updateRequest.Id });
        getResponse.Response.EnsureSuccessStatusCode();
        getResponse.Result.Should().BeEquivalentTo(userPreferences);
    }

    [Fact]
    public async Task UpdateUserPreferences_ReturnsSuccessStatusCode_WhenTheUpdateRequestIsValidContaingLanguageIsoAndOtherSettings()
    {
        // Arrange
        string languageIso = "EN";
        Guid languageId = new Guid("094b3769-68b1-6211-ba2d-6bba92d6a167");
        var updateRequest = new UpdateRequest
        {
            LanguageId = null,
            LanguageIso = languageIso,
            Preferences = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            }
        };

        Dictionary<string, string> userPreferences = new Dictionary<string, string>
        {
            { "LanguageIso",languageIso },
            { "LanguageId", languageId.ToString()},
             { "key1", "value1" },
                { "key2", "value2" }
        };

        // Act
        var updateResponse = await Fixture.PlatformAdmin.POSTAsync<UppdateEndpoint, UpdateRequest, UserPreferencesModel>(updateRequest);

        // Assert
        updateResponse.Response.EnsureSuccessStatusCode();
        updateResponse.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResponse = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, Dictionary<string, string>>(new GetRequest { Id = updateRequest.Id });
        getResponse.Response.EnsureSuccessStatusCode();
        getResponse.Result.Should().BeEquivalentTo(userPreferences);
    }


    [Fact]
    public async Task UpdateUserPreferences_ReturnsNotFound_WhenLanguageIsoIsInvalid()
    {
        // Arrange
        string languageIso = "KZ";
        var updateRequest = new UpdateRequest
        {
            LanguageIso = languageIso
        };

        // Act
        var updateResponse = await Fixture.PlatformAdmin.POSTAsync<UppdateEndpoint, UpdateRequest, Results<NoContent, NotFound<string>>>(updateRequest);

        // Assert
        updateResponse.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateUserPreferences_ReturnsNotFound_WhenLanguageIdIsInvalid()
    {
        // Arrange
        Guid languageId = Guid.Empty;
        var updateRequest = new UpdateRequest
        {
            LanguageId = languageId
        };

        // Act
        var updateResponse = await Fixture.PlatformAdmin.POSTAsync<UppdateEndpoint, UpdateRequest, UserPreferencesModel>(updateRequest);

        // Assert
        updateResponse.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateUserPreferences_ReturnsBadRequest_WhenLanguageIdandLanguageIsoAreMissing()
    {
        // Arrange
        var updateRequest = new UpdateRequest();

        // Act
        var (updateResponse, errors) = await Fixture.PlatformAdmin.POSTAsync<UppdateEndpoint, UpdateRequest, FEProblemDetails>(updateRequest);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors.Errors.Count().Should().Be(2);
        errors.Errors.Any(e => string.Equals(e.Reason, "'Language Iso' must not be empty.")).Should().BeTrue();
        errors.Errors.Any(e => string.Equals(e.Reason, "'Language Id' must not be empty.")).Should().BeTrue();
    }
}
