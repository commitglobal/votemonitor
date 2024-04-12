﻿using Vote.Monitor.Api.Feature.Country;
using Vote.Monitor.Api.Feature.Country.Get;
namespace Vote.Monitor.Api.IntegrationTests.Country;
public class GetEndpointTest : IClassFixture<HttpServerFixture<NoopDataSeeder>>
{
    public HttpServerFixture<NoopDataSeeder> Fixture { get; }

    public GetEndpointTest(HttpServerFixture<NoopDataSeeder> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }


    [Theory]
    [InlineData("b8b09512-ea4c-4a61-9331-304f55324ef7", "IO", "British Indian Ocean Territory (Chagos Archipelago)")]
    [InlineData("6d0c77a7-a4aa-c2bd-2db6-0e2ad2d61f8a", "GH", "Ghana")]
    [InlineData("47804b6a-e705-b925-f4fd-4adf6500180b", "NF", "Norfolk Island")]
    [InlineData("6984f722-6963-d067-d4d4-9fd3ef2edbf6","ZW", "Zimbabwe")]
    [InlineData("899c2a9f-f35d-5a49-a6cd-f92531bb2266", "MF", "Saint Martin")]
    public async Task Should_ReturnCorrectCountry(Guid id, string code, string countryName)
    {
        // Arrange
       var request = new Request
       {
            Id = id
        };  

        // Act
        var (response, result) = await Fixture.PlatformAdmin.GETAsync<Endpoint, Request ,CountryModel>(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Id.Should().Be(id);  
        result.Iso2.Should().Be(code);
        result.Name.Should().Be(countryName);
    }

    [Theory]
    [InlineData("899c2a9f-f35d-5a49-a6cd-f92531112266")]
    public async Task Should_ReturnNotFound_WhenInvalidCountryGuid(Guid id)
    {
        // Arrange
        var request = new Request
        {
            Id = id
        };

        // Act
        var (response, _) = await Fixture.PlatformAdmin.GETAsync<Endpoint, Request, CountryModel>(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
