using Vote.Monitor.Api.Feature.CSO;
using UpdateEndpoint = Vote.Monitor.Api.Feature.CSO.Update.Endpoint;
using UpdateRequest = Vote.Monitor.Api.Feature.CSO.Update.Request;
using CreateEndpoint = Vote.Monitor.Api.Feature.CSO.Create.Endpoint;
using CreateRequest = Vote.Monitor.Api.Feature.CSO.Create.Request;
using GetEndpoint = Vote.Monitor.Api.Feature.CSO.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.CSO.Get.Request;

namespace Vote.Monitor.Api.IntegrationTests.CSO;
public class UpdateEndpointTests : IClassFixture<HttpServerFixture<NoopDataSeeder>>
{
    public HttpServerFixture<NoopDataSeeder> Fixture { get; }

    public UpdateEndpointTests(HttpServerFixture<NoopDataSeeder> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_ActivateCso_WhenValidRequestData()
    {
        // Arrange
        var newCso = new CreateRequest
        {
            Name = "test21"
        };


        var (createRespose, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, CSOModel>(newCso);
        createRespose.IsSuccessStatusCode.Should().BeTrue();
        var updateCSO = new UpdateRequest
        {
            Id = createResult.Id,
            Name = "UpdateTest"
        };

        // Act
        var updateResponse = await Fixture.PlatformAdmin.PUTAsync<UpdateEndpoint, UpdateRequest>(updateCSO);
        // Assert
        updateResponse.IsSuccessStatusCode.Should().BeTrue();
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var request = new GetRequest
        {
            Id = createResult.Id
        };
        var (getResponse, csoModel) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, CSOModel>(request);

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        csoModel.Should().BeEquivalentTo(updateCSO);
        csoModel.Id.Should().Be(createResult.Id);
        csoModel.Name.Should().Be(updateCSO.Name);

    }

    [Fact]
    public async Task Should_BadRequest_WhenNoIdNoName()
    {
        // Arrange
        var csoRequest = new UpdateRequest() { Name = "" };

        // Act
        var (updateResponse, updateErrors) = await Fixture.PlatformAdmin.PUTAsync<UpdateEndpoint, UpdateRequest, FEProblemDetails>(csoRequest);
        // Assert
        updateResponse.IsSuccessStatusCode.Should().BeFalse();
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        updateErrors.Errors.Count().Should().Be(2);
        updateErrors.Errors.Any(e => string.Equals(e.Reason, "'Id' must not be empty.")).Should().BeTrue();
        updateErrors.Errors.Any(e => string.Equals(e.Reason, "'Name' must not be empty.")).Should().BeTrue();
    }
    [Fact]
    public async Task Should_BadRequest_WhenCSONoIdNameInvalid()
    {
        // Arrange
        var csoRequest = new UpdateRequest() { Name = "dk" };

        // Act
        var (updateResponse, updateErrors) = await Fixture.PlatformAdmin.PUTAsync<UpdateEndpoint, UpdateRequest, FEProblemDetails>(csoRequest);
        // Assert
        updateResponse.IsSuccessStatusCode.Should().BeFalse();
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        updateErrors.Errors.Count().Should().Be(2);
        updateErrors.Errors.Any(e => e.Reason == "'Id' must not be empty.").Should().BeTrue();
        updateErrors.Errors.Any(e => e.Reason == "The length of 'Name' must be at least 3 characters. You entered 2 characters.").Should().BeTrue();
    }

    [Fact]
    public async Task Should_NotFound_WhenCSOIdDoesNotExists()
    {
        // Arrange
        var csoRequest = new UpdateRequest()
        {
            Id = Guid.NewGuid(),
            Name = "dkddkkd"
        };

        // Act
        var updateResponse = await Fixture.PlatformAdmin.PUTAsync<UpdateEndpoint, UpdateRequest, CSOModel>(csoRequest);
        // Assert
        updateResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        updateResponse.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_DuplicateName_WhenUpdatedNameAlreadyExists()
    {
        // Arrange
        var newCso1 = new CreateRequest
        {
            Name = "test21"
        };
        var newCso2 = new CreateRequest
        {
            Name = "test22"
        };
        _ = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, CSOModel>(newCso2);

        var (_, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, CSOModel>(newCso1);

        var updateCSO = new UpdateRequest
        {
            Id = createResult.Id,
            Name = "test22"
        };

        // Act
        var (updateResponse, updateErrors) = await Fixture.PlatformAdmin.PUTAsync<UpdateEndpoint, UpdateRequest, FEProblemDetails>(updateCSO);
        // Assert
        updateResponse.IsSuccessStatusCode.Should().BeFalse();
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        updateErrors.Errors.Count().Should().Be(1);
        updateErrors.Errors.First().Reason.Should().Be("A CSO with same name already exists");
    }
}
