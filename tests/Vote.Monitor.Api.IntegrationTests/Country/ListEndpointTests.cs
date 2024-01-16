using Vote.Monitor.Api.Feature.Country;

namespace Vote.Monitor.Api.IntegrationTests.Country;
public class ListEndpointTests:IClassFixture<HttpServerFixture>

{
    public HttpServerFixture Fixture { get; }

    public ListEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_ReturnAllCountries()
    {
        // Arrange &c Act
        var (response, result) = await Fixture.PlatformAdmin.GETAsync<Vote.Monitor.Api.Feature.Country.List.Endpoint, List<CountryModel>>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Count.Should().Be(249);
    }

    [Fact]
    public async Task Should_Check3RandomCountries()
    {
        // Arrange
        List<Tuple<string, string>> testCountries = new List<Tuple<string, string>>
        {
            new Tuple<string, string>("BR", "Brazil"), 
            new Tuple<string, string>("GE", "Georgia"), 
            new Tuple<string, string>("ES", "Spain")
        };
    


        // Act
        var (response, result) = await Fixture.PlatformAdmin.GETAsync<Vote.Monitor.Api.Feature.Country.List.Endpoint, List<CountryModel>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        foreach(var country in testCountries)
        {
            result.Should().Contain(x => x.Iso2 == country.Item1 && x.Name == country.Item2);
        }
    }
}
