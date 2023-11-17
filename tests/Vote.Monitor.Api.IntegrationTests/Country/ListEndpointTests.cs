using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    [Theory]
    [InlineData("BR","Brazil")]
    [InlineData("GE","Georgia")]
    [InlineData("ES","Spain")]
    public async Task Should_ContainCountries(string code,string name)
    {
        // Arrange


        // Act
        var (response, result) = await Fixture.PlatformAdmin.GETAsync<Vote.Monitor.Api.Feature.Country.List.Endpoint, List<CountryModel>>();


        var expected = result.FirstOrDefault(x => x.Iso2 == code && x.Name == name);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        expected.Should().NotBeNull();
    }

    [Theory]
    [InlineData("BR1", "Brazil")]
    [InlineData("GE1", "Georgia")]
    [InlineData("ES1", "Spain")]
    public async Task Should_NotCountries(string code, string name)
    {
        // Arrange


        // Act
        var (response, result) = await Fixture.PlatformAdmin.GETAsync<Vote.Monitor.Api.Feature.Country.List.Endpoint, List<CountryModel>>();


        var expected = result.FirstOrDefault(x => x.Iso2 == code && x.Name == name);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        expected.Should().BeNull();
    }
}
