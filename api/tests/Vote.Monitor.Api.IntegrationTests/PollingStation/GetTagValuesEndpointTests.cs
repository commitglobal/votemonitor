﻿namespace Vote.Monitor.Api.IntegrationTests.PollingStation;

public class GetTagValuesEndpointTests : IClassFixture<HttpServerFixture<NoopDataSeeder>>
{
    public HttpServerFixture<NoopDataSeeder> Fixture { get; }

    public GetTagValuesEndpointTests(HttpServerFixture<NoopDataSeeder> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }

    //[Fact]
    //public async Task Should_Return_TagValues_ForSelectedTag()
    //{
    //    // Arrange
    //    await Fixture.PlatformAdmin.ImportPollingStations(Fixture.ElectionRound.Id);

    //    var request = new GetTagValuesRequest
    //    {
    //        ElectionRoundId = Fixture.ElectionRound.Id,
    //        SelectTag = "Country"
    //    };

    //    // Act
    //    var (response, result) = await Fixture.PlatformAdmin.POSTAsync<GetTagValuesEndpoint, GetTagValuesRequest, List<TagModel>>(request);

    //    // Assert
    //    response.StatusCode.Should().Be(HttpStatusCode.OK);
    //    result.Should().HaveCount(2)
    //        .And.BeEquivalentTo(new[] { new TagModel { Name = "Country", Value = "Poland" }, new TagModel { Name = "Country", Value = "Romania" } });
    //}

    //[Fact]
    //public async Task Should_Return_TagValues_ForSelectedTag_WhenFiltersApplied()
    //{
    //    // Arrange
    //    await Fixture.PlatformAdmin.ImportPollingStations(Fixture.ElectionRound.Id);

    //    var request = new GetTagValuesRequest
    //    {
    //        ElectionRoundId = Fixture.ElectionRound.Id,
    //        SelectTag = "UAT",
    //        Filter = new Dictionary<string, string>
    //        {
    //            {"Country", "Romania"}
    //        }
    //    };

    //    // Act
    //    var (response, result) = await Fixture.PlatformAdmin.POSTAsync<GetTagValuesEndpoint, GetTagValuesRequest, List<TagModel>>(request);

    //    // Assert
    //    response.StatusCode.Should().Be(HttpStatusCode.OK);
    //    result.Should().HaveCount(7)
    //        .And.BeEquivalentTo(new[]
    //        {
    //            new TagModel { Name = "UAT", Value = "MUNICIPIUL ALBA IULIA" },
    //            new TagModel { Name = "UAT", Value = "MUNICIPIUL AIUD" },
    //            new TagModel { Name = "UAT", Value = "MUNICIPIUL BLAJ" },
    //            new TagModel { Name = "UAT", Value = "MUNICIPIUL SEBEŞ" },
    //            new TagModel { Name = "UAT", Value = "ORAŞ ABRUD" },
    //            new TagModel { Name = "UAT", Value = "ORAŞ BAIA DE ARIEŞ" },
    //            new TagModel { Name = "UAT", Value = "ORAŞ CÂMPENI" }
    //        });
    //}

    //[Fact]
    //public async Task Should_Return_EmptyList_WhenNoRowsInDbFroAppliedFilters()
    //{
    //    // Arrange
    //    await Fixture.PlatformAdmin.ImportPollingStations(Fixture.ElectionRound.Id);

    //    var request = new GetTagValuesRequest
    //    {
    //        ElectionRoundId = Fixture.ElectionRound.Id,
    //        SelectTag = "UAT",
    //        Filter = new Dictionary<string, string>
    //        {
    //            {"Country", "Poland"},
    //            {"UAT", "MUNICIPIUL AIUD"}
    //        }
    //    };

    //    // Act
    //    var (response, result) = await Fixture.PlatformAdmin.POSTAsync<GetTagValuesEndpoint, GetTagValuesRequest, List<TagModel>>(request);

    //    // Assert
    //    response.StatusCode.Should().Be(HttpStatusCode.OK);
    //    result.Should().BeEmpty();
    //}
}
