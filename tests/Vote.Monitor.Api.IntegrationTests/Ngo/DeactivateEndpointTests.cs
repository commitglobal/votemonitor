using Vote.Monitor.Api.Feature.Ngo;
using Vote.Monitor.Domain.Entities.NgoAggregate;
using ActivateEndpoint = Vote.Monitor.Api.Feature.Ngo.Activate.Endpoint;
using ActivateRequest = Vote.Monitor.Api.Feature.Ngo.Activate.Request;
using CreateEndpoint = Vote.Monitor.Api.Feature.Ngo.Create.Endpoint;
using CreateRequest = Vote.Monitor.Api.Feature.Ngo.Create.Request;
using GetEndpoint = Vote.Monitor.Api.Feature.Ngo.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.Ngo.Get.Request;
using DeactivateEndpoint = Vote.Monitor.Api.Feature.Ngo.Deactivate.Endpoint;
using DeactivateRequest = Vote.Monitor.Api.Feature.Ngo.Deactivate.Request;


namespace Vote.Monitor.Api.IntegrationTests.Ngo;
public class DeactivateEndpointTests : IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public DeactivateEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_DeactivateNgo_WhenValidRequestData()
    {
        // Arrange
        var newNgo = new CreateRequest
        {
            Name = "test1"
        };

        var (_, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, NgoModel>(newNgo);

        var deactivateRequest = new DeactivateRequest
        {
            Id = createResult.Id
        };

        // Act
        var deactivateResponse = await Fixture.PlatformAdmin.POSTAsync<DeactivateEndpoint, DeactivateRequest, NgoModel>(deactivateRequest);

        // Assert
        deactivateResponse.Response.IsSuccessStatusCode.Should().BeTrue();

        var request = new GetRequest
        {
            Id = createResult.Id
        };
        var (getResponse, ngoModel) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, NgoModel>(request);

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        ngoModel.Should().BeEquivalentTo(newNgo);
        ngoModel.Id.Should().Be(createResult.Id);
        ngoModel.Status.Should().Be(NgoStatus.Deactivated);

    }

    [Fact]
    public async Task Should_DeactivateNgo_WhenValidRequestDataAndNgoActivated()
    {
        // Arrange
        var newNgo = new CreateRequest
        {
            Name = "test12"
        };

        var (_, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, NgoModel>(newNgo);

        _ = await Fixture.PlatformAdmin.POSTAsync<ActivateEndpoint, ActivateRequest, NgoModel>(
            new ActivateRequest
            {
                Id = createResult.Id
            });

        var deactivateNgoRequest = new DeactivateRequest
        {
            Id = createResult.Id
        };
        // Act
        var activateResponse = await Fixture.PlatformAdmin.POSTAsync<DeactivateEndpoint, DeactivateRequest, NgoModel>(deactivateNgoRequest);
        // Assert
        activateResponse.Response.IsSuccessStatusCode.Should().BeTrue();

        var (getResponse, ngoModel) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, NgoModel>(new GetRequest
        {
            Id = createResult.Id
        });

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        ngoModel.Should().BeEquivalentTo(newNgo);
        ngoModel.Id.Should().Be(createResult.Id);
        ngoModel.Status.Should().Be(NgoStatus.Deactivated);

    }

    [Fact]
    public async Task Should_BadRequest_WhenNgoIdEmpty()
    {
        // Arrange
        var deactivateRequest = new DeactivateRequest();

        // Act
        var deactivateResponse = await Fixture.PlatformAdmin.POSTAsync<DeactivateEndpoint, DeactivateRequest, NgoModel>(deactivateRequest);
        // Assert
        deactivateResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        deactivateResponse.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_NotFound_WhenNgoIdDoesNotExists()
    {
        // Arrange
        var deactivateRequest = new DeactivateRequest
        {
            Id = Guid.NewGuid()
        };

        // Act
        var deactivateResponse = await Fixture.PlatformAdmin.POSTAsync<DeactivateEndpoint, DeactivateRequest, NgoModel>(deactivateRequest);
        // Assert
        deactivateResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        deactivateResponse.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
