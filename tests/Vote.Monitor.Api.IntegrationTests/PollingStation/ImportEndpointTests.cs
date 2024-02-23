namespace Vote.Monitor.Api.IntegrationTests.PollingStation;

public class ImportEndpointTests : IClassFixture<HttpServerFixture<NoopDataSeeder>>
{
    public HttpServerFixture<NoopDataSeeder> Fixture { get; }

    public ImportEndpointTests(HttpServerFixture<NoopDataSeeder> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    [Fact]
    public async Task Should_ImportPollingStations_WhenValidCsvProvided()
    {
        // Arrange & Act
        var (importResult, importResponse) = await Fixture.PlatformAdmin.ImportPollingStations();

        // Assert
        importResult.IsSuccessStatusCode.Should().BeTrue();
        importResponse.RowsImported.Should().Be(176);
    }
}
