using Vote.Monitor.Api.Feature.Ngo;
using CreateEndpoint = Vote.Monitor.Api.Feature.Ngo.Create.Endpoint;
using CreateRequest = Vote.Monitor.Api.Feature.Ngo.Create.Request;

using GetEndpoint = Vote.Monitor.Api.Feature.Ngo.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.Ngo.Get.Request;

namespace Vote.Monitor.Api.IntegrationTests.Ngo;


[Collection("IntegrationTests")]
public class CreateEndpointTests : IClassFixture<HttpServerFixture<NoopDataSeeder>>
{
    public HttpServerFixture<NoopDataSeeder> Fixture { get; }

    public CreateEndpointTests(HttpServerFixture<NoopDataSeeder> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_CreateNgo_WhenValidRequestData()
    {
        // Arrange
        var newNgo = new CreateRequest
        {
            Name = "test1"
        };

        // Act
        var (createResponse, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, NgoModel>(newNgo);

        // Assert
        createResponse.IsSuccessStatusCode.Should().BeTrue();
        createResult.Id.Should().NotBeEmpty();

        var request = new GetRequest
        {
            Id = createResult.Id
        };
        var (getResponse, pollingStation) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, NgoModel>(request);

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        pollingStation.Should().BeEquivalentTo(newNgo);
        pollingStation.Id.Should().Be(createResult.Id);
    }

    [Fact]
    public async Task Should_NotCreateNgo_WhenInvalidRequestData()
    {
        // Arrange
        var newNgo = new CreateRequest
        {
            Name = ""
        };

        // Act
        var (createResponse, errorResponse) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, FEProblemDetails>(newNgo);

        // Assert
        createResponse.IsSuccessStatusCode.Should().BeFalse();
        errorResponse.Errors.Count().Should().Be(1);
    }


    [Fact]
    public async Task Should_NotCreateNgo_WhenDuplicate()
    {
        // Arrange
        var createRequest = new CreateRequest
        {
            Name = "test1"
        };

        _ = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, FEProblemDetails>(createRequest);

        // Act
        var (createResponse, errorResponse) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, FEProblemDetails>(createRequest);
        // Assert
        createResponse.IsSuccessStatusCode.Should().BeFalse();
        errorResponse.Errors.Count().Should().Be(1);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        errorResponse.Errors.First().Reason.Should().Be("A Ngo with same name already exists");
    }

}
