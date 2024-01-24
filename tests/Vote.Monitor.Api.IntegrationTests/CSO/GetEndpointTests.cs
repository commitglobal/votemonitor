using Vote.Monitor.Api.Feature.CSO;
using CreateEndpoint = Vote.Monitor.Api.Feature.CSO.Create.Endpoint;
using CreateRequest = Vote.Monitor.Api.Feature.CSO.Create.Request;
using GetEndpoint = Vote.Monitor.Api.Feature.CSO.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.CSO.Get.Request;

namespace Vote.Monitor.Api.IntegrationTests.CSO;
public class GetEndpointTests : IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public GetEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_GetCso_WhenValidRequestData()
    {
        // Arrange
        var newCso = new CreateRequest
        {
            Name = "test21"
        };
        var (_, createResult) = await Fixture.PlatformAdmin.POSTAsync<CreateEndpoint, CreateRequest, CSOModel>(newCso);
        var getCsoRequest = new GetRequest
        {
            Id = createResult.Id
        };

        // Act
        var (getResponse, csoModel) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, CSOModel>(getCsoRequest);

        // Assert
        getResponse.IsSuccessStatusCode.Should().BeTrue();
        csoModel.Name.Should().BeEquivalentTo(newCso.Name);
        csoModel.Id.Should().Be(createResult.Id);

    }



    [Fact]
    public async Task Should_BadRequest_WhenCSOIdEmpty()
    {
        //Arrange 
        var getCsoRequest = new GetRequest();

        // Act
        var (getResponse, problemDetails) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, FEProblemDetails>(getCsoRequest);

        // Assert
        getResponse.IsSuccessStatusCode.Should().BeFalse();
        getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Errors.Count().Should().Be(1);
        problemDetails.Errors.First().Reason.Should().Be("'Id' must not be empty.");

    }

    [Fact]
    public async Task Should_NotFound_WhenCSOIdDoesNotExists()
    {
        // Arrange
        var getCsoRequest = new GetRequest()
        {
            Id = Guid.NewGuid()
        };

        // Act
        var getResponse = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, FEProblemDetails>(getCsoRequest);

        // Assert
        getResponse.Response.IsSuccessStatusCode.Should().BeFalse();
        getResponse.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
