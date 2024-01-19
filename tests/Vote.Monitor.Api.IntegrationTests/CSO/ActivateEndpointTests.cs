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
public class ActivateEndpointTests : IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public ActivateEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
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
        var newCsoActivate = new ActivateRequest
        {
            Id = createResult.Id
        };

        // Act
        var activateResponse = await Fixture.PlatformAdmin.POSTAsync<ActivateEndpoint, ActivateRequest, CSOModel>(newCsoActivate);
        // Assert
        activateResponse.Response.IsSuccessStatusCode.Should().BeTrue();

        var request = new GetRequest
        {
            Id = createResult.Id
        };
        var (getResponse, csoModel) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, CSOModel>(request);

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        csoModel.Should().BeEquivalentTo(newCso);
        csoModel.Id.Should().Be(createResult.Id);
        csoModel.Status.Should().Be(CSOStatus.Activated);

    }

    [Fact]
    public async Task Should_ActivateCso_WhenValidRequestDataAndCsoDeactivated()
    {
        // Arrange
        var newCso = new CreateRequest
        {
            Name = "test1"
        };

        var (_, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, CSOModel>(newCso);

        _ = await Fixture.PlatformAdmin.POSTAsync<DeactivateEndpoint, DeactivateRequest, CSOModel>(
            new DeactivateRequest
            {
                Id = createResult.Id
            });

        var newCsoActivate = new ActivateRequest
        {
            Id = createResult.Id
        };
        // Act
        var activateResponse = await Fixture.PlatformAdmin.POSTAsync<ActivateEndpoint, ActivateRequest, CSOModel>(newCsoActivate);
        // Assert
        activateResponse.Response.IsSuccessStatusCode.Should().BeTrue();


        var (getResponse, csoModel) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, CSOModel>(new GetRequest
        {
            Id = createResult.Id
        });

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        csoModel.Should().BeEquivalentTo(newCso);
        csoModel.Id.Should().Be(createResult.Id);
        csoModel.Status.Should().Be(CSOStatus.Activated);
    }

    [Fact]
    public async Task Should_BadRequest_WhenCSOIdEmpty()
    {
        // Arrange
        var csoRequest = new ActivateRequest();

        // Act
        var activateResponse = await Fixture.PlatformAdmin.POSTAsync<ActivateEndpoint, ActivateRequest, CSOModel>(csoRequest);
        // Assert
        activateResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        activateResponse.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_NotFound_WhenCSOIdDoesNotExists()
    {
        // Arrange
        var csoRequest = new ActivateRequest()
        { Id = Guid.NewGuid() };

        // Act
        var activateResponse = await Fixture.PlatformAdmin.POSTAsync<ActivateEndpoint, ActivateRequest, CSOModel>(csoRequest);
        // Assert
        activateResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        activateResponse.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
