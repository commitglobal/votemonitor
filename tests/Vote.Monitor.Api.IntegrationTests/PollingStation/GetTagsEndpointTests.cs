namespace Vote.Monitor.Api.IntegrationTests.PollingStation;

public class GetTagsEndpointTests : IClassFixture<HttpServerFixture<NoopDataSeeder>>
{
    public HttpServerFixture<NoopDataSeeder> Fixture { get; }

    public GetTagsEndpointTests(HttpServerFixture<NoopDataSeeder> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    //[Fact]
    //public async Task Should_Return_AllAvailableTags()
    //{
    //    // Arrange
    //    await Fixture.PlatformAdmin.ImportPollingStations(Fixture.ElectionRound.Id);
    //    // Act
    //    var (getTagsResult, tags) = await Fixture.PlatformAdmin.GETAsync<GetTagsEndpoint, GetTagsRequest, List<string>>(
    //    new()
    //    {
    //        ElectionRoundId = Fixture.ElectionRound.Id
    //    });

    //    // Assert
    //    getTagsResult.IsSuccessStatusCode.Should().BeTrue();
    //    tags
    //        .Should()
    //        .HaveCount(7)
    //        .And.BeEquivalentTo("Country", "Gmina", "Powiat", "Województwo", "Judet", "UAT", "Number");
    //}
}
