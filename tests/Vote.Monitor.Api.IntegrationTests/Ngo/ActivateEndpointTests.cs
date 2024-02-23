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
public class ActivateEndpointTests : IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public ActivateEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
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
        var activateRequest = new ActivateRequest
        {
            Id = createResult.Id
        };

        // Act
        var activateResponse = await Fixture.PlatformAdmin.POSTAsync<ActivateEndpoint, ActivateRequest, NgoModel>(activateRequest);
        // Assert
        activateResponse.Response.IsSuccessStatusCode.Should().BeTrue();

        var request = new GetRequest
        {
            Id = createResult.Id
        };
        var (getResponse, ngoModel) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, NgoModel>(request);

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        ngoModel.Should().BeEquivalentTo(createRequest);
        ngoModel.Id.Should().Be(createResult.Id);
        ngoModel.Status.Should().Be(NgoStatus.Activated);

    }

    [Fact]
    public async Task Should_ActivateNgo_WhenValidRequestDataAndNgoDeactivated()
    {
        // Arrange
        var createRequest = new CreateRequest
        {
            Name = "test1"
        };

        var (_, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, NgoModel>(createRequest);

        _ = await Fixture.PlatformAdmin.POSTAsync<DeactivateEndpoint, DeactivateRequest, NgoModel>(
            new DeactivateRequest
            {
                Id = createResult.Id
            });

        var activateRequest = new ActivateRequest
        {
            Id = createResult.Id
        };

        // Act
        var activateResponse = await Fixture.PlatformAdmin.POSTAsync<ActivateEndpoint, ActivateRequest, NgoModel>(activateRequest);

        // Assert
        activateResponse.Response.IsSuccessStatusCode.Should().BeTrue();

        var (getResponse, ngoModel) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, NgoModel>(new GetRequest
        {
            Id = createResult.Id
        });

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        ngoModel.Should().BeEquivalentTo(createRequest);
        ngoModel.Id.Should().Be(createResult.Id);
        ngoModel.Status.Should().Be(NgoStatus.Activated);
    }

    [Fact]
    public async Task Should_BadRequest_WhenNgoIdEmpty()
    {
        // Arrange
        var activateRequest = new ActivateRequest();

        // Act
        var activateResponse = await Fixture.PlatformAdmin.POSTAsync<ActivateEndpoint, ActivateRequest, NgoModel>(activateRequest);

        // Assert
        activateResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        activateResponse.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_NotFound_WhenNgoIdDoesNotExists()
    {
        // Arrange
        var activateRequest = new ActivateRequest { Id = Guid.NewGuid() };

        // Act
        var activateResponse = await Fixture.PlatformAdmin.POSTAsync<ActivateEndpoint, ActivateRequest, NgoModel>(activateRequest);

        // Assert
        activateResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        activateResponse.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
