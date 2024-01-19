using Vote.Monitor.Api.Feature.CSO;
using ActivateEndpoint = Vote.Monitor.Api.Feature.CSO.Activate.Endpoint;
using ActivateRequest = Vote.Monitor.Api.Feature.CSO.Activate.Request;
using CreateEndpoint = Vote.Monitor.Api.Feature.CSO.Create.Endpoint;
using CreateRequest = Vote.Monitor.Api.Feature.CSO.Create.Request;
using GetEndpoint = Vote.Monitor.Api.Feature.CSO.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.CSO.Get.Request;
using CSOStatus = Vote.Monitor.Domain.Entities.CSOAggregate.CSOStatus;
using DeactivateEndpoint = Vote.Monitor.Api.Feature.CSO.Deactivate.Endpoint;
using DeactivateRequest = Vote.Monitor.Api.Feature.CSO.Deactivate.Request;


namespace Vote.Monitor.Api.IntegrationTests.CSO;
public class DeactivateEndpointTests : IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public DeactivateEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_DeactivateCso_WhenValidRequestData()
    {
        // Arrange
        var newCso = new CreateRequest
        {
            Name = "test1"
        };


        var (_, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, CSOModel>(newCso);

        var newCsoActivate = new DeactivateRequest
        {
            Id = createResult.Id
        };

        // Act
        var deactivateResponse = await Fixture.PlatformAdmin.POSTAsync<DeactivateEndpoint, DeactivateRequest, CSOModel>(newCsoActivate);
        // Assert
        deactivateResponse.Response.IsSuccessStatusCode.Should().BeTrue();

        var request = new GetRequest
        {
            Id = createResult.Id
        };
        var (getResponse, csoModel) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, CSOModel>(request);

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        csoModel.Should().BeEquivalentTo(newCso);
        csoModel.Id.Should().Be(createResult.Id);
        csoModel.Status.Should().Be(CSOStatus.Deactivated);

    }

    [Fact]
    public async Task Should_DeactivateCso_WhenValidRequestDataAndCsoActivated()
    {
        // Arrange
        var newCso = new CreateRequest
        {
            Name = "test12"
        };


        var (_, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, CSOModel>(newCso);

        _ = await Fixture.PlatformAdmin.POSTAsync<ActivateEndpoint, ActivateRequest, CSOModel>(
            new ActivateRequest
            {
                Id = createResult.Id
            });

        var deactivateCso = new DeactivateRequest
        {
            Id = createResult.Id
        };
        // Act
        var activateResponse = await Fixture.PlatformAdmin.POSTAsync<DeactivateEndpoint, DeactivateRequest, CSOModel>(deactivateCso);
        // Assert
        activateResponse.Response.IsSuccessStatusCode.Should().BeTrue();


        var (getResponse, csoModel) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, CSOModel>(new GetRequest
        {
            Id = createResult.Id
        });

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        csoModel.Should().BeEquivalentTo(newCso);
        csoModel.Id.Should().Be(createResult.Id);
        csoModel.Status.Should().Be(CSOStatus.Deactivated);

    }

    [Fact]
    public async Task Should_BadRequest_WhenCSOIdEmpty()
    {
        // Arrange
        var csoRequest = new DeactivateRequest();

        // Act
        var deaactivateResponse = await Fixture.PlatformAdmin.POSTAsync<DeactivateEndpoint, DeactivateRequest, CSOModel>(csoRequest);
        // Assert
        deaactivateResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        deaactivateResponse.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_NotFound_WhenCSOIdDoesNotExists()
    {
        // Arrange
        var csoRequest = new DeactivateRequest()
        { Id = Guid.NewGuid() };

        // Act
        var deactivateResponse = await Fixture.PlatformAdmin.POSTAsync<DeactivateEndpoint, DeactivateRequest, CSOModel>(csoRequest);
        // Assert
        deactivateResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        deactivateResponse.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
