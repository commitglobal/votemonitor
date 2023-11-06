using Vote.Monitor.Api.Feature.PollingStation.List;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.PollingStation.IntegrationTests.EndpointsTests;

public class ListEndpointTests : IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public ListEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_UseDefaultPagination_WhenNoFilterAndNoPagination()
    {
        // Arrange
        await Fixture.PlatformAdmin.ImportPollingStations();

        // Act
        var (response, result) = await Fixture.PlatformAdmin.POSTAsync<Endpoint, Request, PagedResponse<PollingStationModel>>(new Request());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.TotalCount.Should().Be(176);
        result.PageSize.Should().Be(10);
        result.CurrentPage.Should().Be(1);
        result.Items.Should().HaveCount(100);
    }

    [Fact]
    public async Task Should_ListDataPaginated()
    {
        // Arrange
        await Fixture.PlatformAdmin.ImportPollingStations();

        var request = new Request
        {
            PageSize = 15,
            AddressFilter = "ALBA IULIA",
            Filter = new Dictionary<string, string>
            {
                {"Country", "Romania"}
            }
        };

        // Act
        var (response, result) = await Fixture.PlatformAdmin.POSTAsync<Endpoint, Request, PagedResponse<PollingStationModel>>(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.TotalCount.Should().Be(43);
        result.PageSize.Should().Be(15);
        result.CurrentPage.Should().Be(1);
        result.Items.Should().HaveCount(15);
    }
}
