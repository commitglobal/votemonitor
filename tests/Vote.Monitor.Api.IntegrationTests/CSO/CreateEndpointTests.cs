using Vote.Monitor.Api.Feature.CSO;
using CreateEndpoint = Vote.Monitor.Api.Feature.CSO.Create.Endpoint;
using CreateRequest = Vote.Monitor.Api.Feature.CSO.Create.Request;

using GetEndpoint = Vote.Monitor.Api.Feature.CSO.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.CSO.Get.Request;

namespace Vote.Monitor.Api.IntegrationTests.CSO;

public class CreateEndpointTests : IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public CreateEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_CreateCso_WhenValidRequestData()
    {
        // Arrange
        var newCso = new CreateRequest
        {
            Name = "test1"
        };

        // Act
        var (createResponse, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, CSOModel>(newCso);

        // Assert
        createResponse.IsSuccessStatusCode.Should().BeTrue();
        createResult.Id.Should().NotBeEmpty();

        var request = new GetRequest
        {
            Id = createResult.Id
        };
        var (getResponse, pollingStation) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, CSOModel>(request);

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        pollingStation.Should().BeEquivalentTo(newCso);
        pollingStation.Id.Should().Be(createResult.Id);
    }

    [Fact]
    public async Task Should_NotCreateCso_WhenInvalidRequestData()
    {
        // Arrange
        var newCSO = new CreateRequest
        {
            Name = ""
        };

        // Act
        var (createResponse, errorResponse) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, FEProblemDetails>(newCSO);

        // Assert
        createResponse.IsSuccessStatusCode.Should().BeFalse();
        errorResponse.Errors.Count().Should().Be(1);
    }


    [Fact]
    public async Task Should_NotCreateCso_WhenDuplicate()
    {
        // Arrange
        var newCSO = new CreateRequest
        {
            Name = "test1"
        };


        _ = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, FEProblemDetails>(newCSO);

        // Act
        var (createResponse, errorResponse) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, FEProblemDetails>(newCSO);
        // Assert
        createResponse.IsSuccessStatusCode.Should().BeFalse();
        errorResponse.Errors.Count().Should().Be(1);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        errorResponse.Errors.First().Reason.Should().Be("A CSO with same name already exists");

    }

}
