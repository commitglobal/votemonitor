using Vote.Monitor.Api.Feature.Ngo;
using CreateEndpoint = Vote.Monitor.Api.Feature.Ngo.Create.Endpoint;
using CreateRequest = Vote.Monitor.Api.Feature.Ngo.Create.Request;
using GetEndpoint = Vote.Monitor.Api.Feature.Ngo.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.Ngo.Get.Request;

namespace Vote.Monitor.Api.IntegrationTests.Ngo;
public class GetEndpointTests : IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public GetEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_GetNgo_WhenValidRequestData()
    {
        // Arrange
        var newNgo = new CreateRequest
        {
            Name = "test21"
        };
        var (_, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, NgoModel>(newNgo);
        var getNgoRequest = new GetRequest
        {
            Id = createResult.Id
        };

        // Act
        var (getResponse, ngoModel) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, NgoModel>(getNgoRequest);

        // Assert
        getResponse.IsSuccessStatusCode.Should().BeTrue();
        ngoModel.Name.Should().BeEquivalentTo(newNgo.Name);
        ngoModel.Id.Should().Be(createResult.Id);
    }



    [Fact]
    public async Task Should_BadRequest_WhenNgoIdEmpty()
    {
        //Arrange 
        var request = new GetRequest();

        // Act
        var (getResponse, problemDetails) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, FEProblemDetails>(request);

        // Assert
        getResponse.IsSuccessStatusCode.Should().BeFalse();
        getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Errors.Count().Should().Be(1);
        problemDetails.Errors.First().Reason.Should().Be("'Id' must not be empty.");
    }

    [Fact]
    public async Task Should_NotFound_WhenNgoIdDoesNotExists()
    {
        // Arrange
        var request = new GetRequest
        {
            Id = Guid.NewGuid()
        };

        // Act
        var getResponse = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, FEProblemDetails>(request);

        // Assert
        getResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        getResponse.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
