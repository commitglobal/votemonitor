using Vote.Monitor.Api.Feature.CSO;
using DeleteEndpoint = Vote.Monitor.Api.Feature.CSO.Delete.Endpoint;
using DeleteRequest = Vote.Monitor.Api.Feature.CSO.Delete.Request;
using CreateEndpoint = Vote.Monitor.Api.Feature.CSO.Create.Endpoint;
using CreateRequest = Vote.Monitor.Api.Feature.CSO.Create.Request;
using GetEndpoint = Vote.Monitor.Api.Feature.CSO.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.CSO.Get.Request;


namespace Vote.Monitor.Api.IntegrationTests.CSO;
public class DeleteEndpointTests : IClassFixture<HttpServerFixture<NoopDataSeeder>>
{
    public HttpServerFixture<NoopDataSeeder> Fixture { get; }

    public DeleteEndpointTests(HttpServerFixture<NoopDataSeeder> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_DeleteCso_WhenValidRequestData()
    {
        // Arrange
        var newCso = new CreateRequest
        {
            Name = "test21"
        };


        var (createRespose, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, CSOModel>(newCso);
        createRespose.IsSuccessStatusCode.Should().BeTrue();
        var deleteCso = new DeleteRequest
        {
            Id = createResult.Id
        };

        // Act
        var deleteResponse = await Fixture.PlatformAdmin.DELETEAsync<DeleteEndpoint, DeleteRequest>(deleteCso);
       
        // Assert
        deleteResponse.IsSuccessStatusCode.Should().BeTrue();
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var request = new GetRequest
        {
            Id = createResult.Id
        };
        var getResponse = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, CSOModel>(request);

        getResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        getResponse.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);


    }


    [Fact]
    public async Task Should_BadRequest_WhenCSOIdEmpty()
    {
        // Arrange
        var csoRequest = new DeleteRequest();

        // Act
        var deleteResponse = await Fixture.PlatformAdmin.DELETEAsync<DeleteEndpoint, DeleteRequest, CSOModel>(csoRequest);
        // Assert
        deleteResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        deleteResponse.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_NotFound_WhenCSOIdDoesNotExists()
    {
        // Arrange
        var csoRequest = new DeleteRequest()
        { Id = Guid.NewGuid() };

        // Act
        var deleteResponse = await Fixture.PlatformAdmin.DELETEAsync<DeleteEndpoint, DeleteRequest, CSOModel>(csoRequest);
        // Assert
        deleteResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        deleteResponse.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
