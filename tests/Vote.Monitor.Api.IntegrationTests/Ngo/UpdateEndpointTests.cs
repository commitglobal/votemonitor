using Vote.Monitor.Api.Feature.Ngo;
using UpdateEndpoint = Vote.Monitor.Api.Feature.Ngo.Update.Endpoint;
using UpdateRequest = Vote.Monitor.Api.Feature.Ngo.Update.Request;
using CreateEndpoint = Vote.Monitor.Api.Feature.Ngo.Create.Endpoint;
using CreateRequest = Vote.Monitor.Api.Feature.Ngo.Create.Request;
using GetEndpoint = Vote.Monitor.Api.Feature.Ngo.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.Ngo.Get.Request;

namespace Vote.Monitor.Api.IntegrationTests.Ngo;
public class UpdateEndpointTests : IClassFixture<HttpServHttpServerFixture<NoopDataSeeder>erFixture>
{
    public HttpServerFixture<NoopDataSeeder> Fixture { get; }

    public UpdateEndpointTests(HttpServerFixture<NoopDataSeeder> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_ActivateNgo_WhenValidRequestData()
    {
        // Arrange
        var createRequest = new CreateRequest
        {
            Name = "test21"
        };

        var (createResponse, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, NgoModel>(createRequest);
        createResponse.IsSuccessStatusCode.Should().BeTrue();

        // Act
        var updateNgoRequest = new UpdateRequest
        {
            Id = createResult.Id,
            Name = "UpdateTest"
        };
        var updateResponse = await Fixture.PlatformAdmin.PUTAsync<UpdateEndpoint, UpdateRequest, NgoModel>(updateNgoRequest);
      
        // Assert
        updateResponse.IsSuccessStatusCode.Should().BeTrue();
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var request = new GetRequest
        {
            Id = createResult.Id
        };
        var (getResponse, ngoModel) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, NgoModel>(request);

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        ngoModel.Should().BeEquivalentTo(updateNgoRequest);
        ngoModel.Id.Should().Be(createResult.Id);
        ngoModel.Name.Should().Be(updateNgoRequest.Name);

    }

    [Fact]
    public async Task Should_BadRequest_WhenNoIdNoName()
    {
        // Arrange
        var updateRequest = new UpdateRequest { Name = "" };

        // Act
        var (updateResponse, updateErrors) = await Fixture.PlatformAdmin.PUTAsync<UpdateEndpoint, UpdateRequest, FEProblemDetails>(updateRequest);
        // Assert
        updateResponse.IsSuccessStatusCode.Should().BeFalse();
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        updateErrors.Errors.Count().Should().Be(2);
        updateErrors.Errors.Any(e => string.Equals(e.Reason, "'Id' must not be empty.")).Should().BeTrue();
        updateErrors.Errors.Any(e => string.Equals(e.Reason, "'Name' must not be empty.")).Should().BeTrue();
    }
    [Fact]
    public async Task Should_BadRequest_WhenNgoNoIdNameInvalid()
    {
        // Arrange
        var updateRequest = new UpdateRequest { Name = "dk" };

        // Act
        var (updateResponse, updateErrors) = await Fixture.PlatformAdmin.PUTAsync<UpdateEndpoint, UpdateRequest, FEProblemDetails>(updateRequest);
        // Assert
        updateResponse.IsSuccessStatusCode.Should().BeFalse();
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        updateErrors.Errors.Count().Should().Be(2);
        updateErrors.Errors.Any(e => e.Reason == "'Id' must not be empty.").Should().BeTrue();
        updateErrors.Errors.Any(e => e.Reason == "The length of 'Name' must be at least 3 characters. You entered 2 characters.").Should().BeTrue();
    }

    [Fact]
    public async Task Should_NotFound_WhenNgoIdDoesNotExists()
    {
        // Arrange
        var updateRequest = new UpdateRequest
        {
            Id = Guid.NewGuid(),
            Name = "dkddkkd"
        };

        // Act
        var updateResponse = await Fixture.PlatformAdmin.PUTAsync<UpdateEndpoint, UpdateRequest, NgoModel>(updateRequest);
        // Assert
        updateResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        updateResponse.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_DuplicateName_WhenUpdatedNameAlreadyExists()
    {
        // Arrange
        var createNgo1Request = new CreateRequest
        {
            Name = "test21"
        };

        var createNgo2Request = new CreateRequest
        {
            Name = "test22"
        };

        _ = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, NgoModel>(createNgo2Request);

        var (_, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, NgoModel>(createNgo1Request);

        var updateRequest = new UpdateRequest
        {
            Id = createResult.Id,
            Name = "test22"
        };

        // Act
        var (updateResponse, updateErrors) = await Fixture.PlatformAdmin.PUTAsync<UpdateEndpoint, UpdateRequest, FEProblemDetails>(updateRequest);
        // Assert
        updateResponse.IsSuccessStatusCode.Should().BeFalse();
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        updateErrors.Errors.Count().Should().Be(1);
        updateErrors.Errors.First().Reason.Should().Be("A ngo with same name already exists");
    }
}
