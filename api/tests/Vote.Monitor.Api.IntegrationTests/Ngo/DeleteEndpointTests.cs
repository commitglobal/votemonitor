using Vote.Monitor.Api.Feature.Ngo;
using DeleteEndpoint = Vote.Monitor.Api.Feature.Ngo.Delete.Endpoint;
using DeleteRequest = Vote.Monitor.Api.Feature.Ngo.Delete.Request;
using CreateEndpoint = Vote.Monitor.Api.Feature.Ngo.Create.Endpoint;
using CreateRequest = Vote.Monitor.Api.Feature.Ngo.Create.Request;
using GetEndpoint = Vote.Monitor.Api.Feature.Ngo.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.Ngo.Get.Request;

namespace Vote.Monitor.Api.IntegrationTests.Ngo;
public class DeleteEndpointTests : IClassFixture<HttpServerFixture<NoopDataSeeder>>
{
    public HttpServerFixture<NoopDataSeeder> Fixture { get; }

    public DeleteEndpointTests(HttpServerFixture<NoopDataSeeder> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_DeleteNgo_WhenValidRequestData()
    {
        // Arrange
        var createRequest = new CreateRequest
        {
            Name = "test21"
        };

        var (createResponse, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, NgoModel>(createRequest);
        createResponse.IsSuccessStatusCode.Should().BeTrue();

        // Act
        var deleteRequest = new DeleteRequest
        {
            Id = createResult.Id
        };

        // Act
        var deleteResponse = await Fixture.PlatformAdmin.DELETEAsync<DeleteEndpoint, DeleteRequest>(deleteRequest);

        // Assert
        deleteResponse.IsSuccessStatusCode.Should().BeTrue();
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var request = new GetRequest
        {
            Id = createResult.Id
        };
        var getResponse = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, NgoModel>(request);

        getResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        getResponse.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task Should_BadRequest_WhenNgoIdEmpty()
    {
        // Arrange
        var deleteRequest = new DeleteRequest();

        // Act
        var deleteResponse = await Fixture.PlatformAdmin.DELETEAsync<DeleteEndpoint, DeleteRequest, NgoModel>(deleteRequest);

        // Assert
        deleteResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        deleteResponse.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_NotFound_WhenNgoIdDoesNotExists()
    {
        // Arrange
        var deleteRequest = new DeleteRequest { Id = Guid.NewGuid() };

        // Act
        var deleteResponse = await Fixture.PlatformAdmin.DELETEAsync<DeleteEndpoint, DeleteRequest, NgoModel>(deleteRequest);

        // Assert
        deleteResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        deleteResponse.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
