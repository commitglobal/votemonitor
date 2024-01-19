using Vote.Monitor.Api.Feature.CSO;
using ListEndpoint = Vote.Monitor.Api.Feature.CSO.List.Endpoint;
using ListRequest = Vote.Monitor.Api.Feature.CSO.List.Request;
using SortOrder = Vote.Monitor.Core.Models.SortOrder;
using CSOStatus = Vote.Monitor.Domain.Entities.CSOAggregate.CSOStatus;

namespace Vote.Monitor.Api.IntegrationTests.CSO;
public class ListEndpointTests : IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public ListEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
        //Task.Run(async () => ).Wait();
    }

    [Fact]
    public async Task Should_ListCso_WhenValidRequestDataNoFilteringNoSortingPageSize100Page1()
    {

        // Arrange
        await Fixture.PlatformAdmin.GenerateCSOTestData(Fixture.OutputHelper!);


        var listRequest = new ListRequest
        {
            NameFilter = "",
            Status = null,
            PageNumber = 1,
            PageSize = 100,
            SortColumnName = null,
            SortOrder = null
        };


        // Act
        var (lisResponse, result) = await Fixture.PlatformAdmin.GETAsync<ListEndpoint, ListRequest, PagedResponse<CSOModel>>(listRequest);

        // Assert
        lisResponse.IsSuccessStatusCode.Should().BeTrue();
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(100);
        result.TotalCount.Should().Be(40);


    }

    [Fact]
    public async Task Should_ListCso_WhenValidRequestDataNoFilteringNoSortingPageSize10Page1()
    {

        // Arrange
        await Fixture.PlatformAdmin.GenerateCSOTestData(Fixture.OutputHelper!);

        var listRequest = new ListRequest
        {
            NameFilter = "",
            Status = null,
            PageNumber = 1,
            PageSize = 10,
            SortColumnName = null,
            SortOrder = null
        };

        // Act
        var (lisResponse, result) = await Fixture.PlatformAdmin.GETAsync<ListEndpoint, ListRequest, PagedResponse<CSOModel>>(listRequest);

        // Assert
        lisResponse.IsSuccessStatusCode.Should().BeTrue();
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(40);
        result.Items.Count.Should().Be(10);


    }


    [Fact]
    public async Task Should_ListCso_WhenValidRequestDataNoFilteringNoSortingPageSize16Page3()
    {

        // Arrange
        await Fixture.PlatformAdmin.GenerateCSOTestData(Fixture.OutputHelper!);

        var listRequest = new ListRequest
        {
            NameFilter = "",
            Status = null,
            PageNumber = 3,
            PageSize = 16,
            SortColumnName = null,
            SortOrder = null
        };

        // Act
        var (lisResponse, result) = await Fixture.PlatformAdmin.GETAsync<ListEndpoint, ListRequest, PagedResponse<CSOModel>>(listRequest);

        // Assert
        lisResponse.IsSuccessStatusCode.Should().BeTrue();
        result.CurrentPage.Should().Be(3);
        result.PageSize.Should().Be(16);
        result.TotalCount.Should().Be(40);
        result.Items.Count.Should().Be(8);

    }

    [Fact]
    public async Task Should_ListCso_WhenValidRequestDataFiltering_Name_18_NoSortingPageSize10Page1()
    {

        // Arrange
        await Fixture.PlatformAdmin.GenerateCSOTestData(Fixture.OutputHelper!);

        var listRequest = new ListRequest
        {
            NameFilter = "18",
            Status = null,
            PageNumber = 1,
            PageSize = 10,
            SortColumnName = null,
            SortOrder = null
        };

        // Act
        var (lisResponse, result) = await Fixture.PlatformAdmin.GETAsync<ListEndpoint, ListRequest, PagedResponse<CSOModel>>(listRequest);

        // Assert
        lisResponse.IsSuccessStatusCode.Should().BeTrue();
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(2);
        result.Items.Count.Should().Be(2);

    }

    [Fact]
    public async Task Should_ListCso_WhenValidRequestDataFiltering_Name_Act_NoSortingPageSize10Page1()
    {
        // Arrange
        await Fixture.PlatformAdmin.GenerateCSOTestData(Fixture.OutputHelper!);

        var listRequest = new ListRequest
        {
            NameFilter = "Act",
            Status = null,
            PageNumber = 1,
            PageSize = 10,
            SortColumnName = null,
            SortOrder = null
        };


        // Act
        var (lisResponse, result) = await Fixture.PlatformAdmin.GETAsync<ListEndpoint, ListRequest, PagedResponse<CSOModel>>(listRequest);

        // Assert
        lisResponse.IsSuccessStatusCode.Should().BeTrue();
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(20);
        result.Items.Count.Should().Be(10);
    }


    [Fact]
    public async Task Should_ListCso_WhenValidRequestDataFiltering_Name_Act_Sorting_Name_Desc_PageSize10Page1()
    {
        // Arrange
        await Fixture.PlatformAdmin.GenerateCSOTestData(Fixture.OutputHelper!);

        var listRequest = new ListRequest
        {
            NameFilter = "Act",
            Status = null,
            PageNumber = 1,
            PageSize = 10,
            SortColumnName = "Name",
            SortOrder = SortOrder.Desc
        };


        // Act
        var (lisResponse, result) = await Fixture.PlatformAdmin.GETAsync<ListEndpoint, ListRequest, PagedResponse<CSOModel>>(listRequest);

        // Assert
        lisResponse.IsSuccessStatusCode.Should().BeTrue();
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(20);
        result.Items.Count.Should().Be(10);
        result.Items[0].Name.Should().Be("Activated9");
    }

    [Fact]
    public async Task Should_ListCso_WhenValidRequestDataFiltering_Name_Act_Sorting_Name_ASC_PageSize10Page1()
    {

        // Arrange
        await Fixture.PlatformAdmin.GenerateCSOTestData(Fixture.OutputHelper!);

        var listRequest = new ListRequest
        {
            NameFilter = "Act",
            Status = null,
            PageNumber = 1,
            PageSize = 10,
            SortColumnName = "Name",
            SortOrder = SortOrder.Asc
        };

        // Act
        var (lisResponse, result) = await Fixture.PlatformAdmin.GETAsync<ListEndpoint, ListRequest, PagedResponse<CSOModel>>(listRequest);

        // Assert
        lisResponse.IsSuccessStatusCode.Should().BeTrue();
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(20);
        result.Items.Count.Should().Be(10);
        result.Items[0].Name.Should().Be("Activated0");

    }


    [Fact]
    public async Task Should_ListCso_WhenValidRequestDataFiltering_Name_1_Sorting_Name_ASC_PageSize10Page1()
    {

        // Arrange
        await Fixture.PlatformAdmin.GenerateCSOTestData(Fixture.OutputHelper!);

        var listRequest = new ListRequest
        {
            NameFilter = "1",
            Status = null,
            PageNumber = 1,
            PageSize = 10,
            SortColumnName = "Name",
            SortOrder = SortOrder.Asc
        };


        // Act
        var (lisResponse, result) = await Fixture.PlatformAdmin.GETAsync<ListEndpoint, ListRequest, PagedResponse<CSOModel>>(listRequest);

        // Assert
        lisResponse.IsSuccessStatusCode.Should().BeTrue();
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(22);
        result.Items.Count.Should().Be(10);
        // should be A1,  A10, A11, A12,A13
        result.Items[0].Name.Should().Be($"Activated1");
        result.Items[1].Name.Should().Be($"Activated10");
        result.Items[2].Name.Should().Be($"Activated11");
        result.Items[3].Name.Should().Be($"Activated12");
        result.Items[4].Name.Should().Be($"Activated13");

    }

    [Fact]
    public async Task Should_ListCso_WhenValidRequestDataFiltering_Name_1_Sorting_Name_Desc_PageSize10Page1()
    {

        // Arrange
        await Fixture.PlatformAdmin.GenerateCSOTestData(Fixture.OutputHelper!);

        var listRequest = new ListRequest
        {
            NameFilter = "1",
            Status = null,
            PageNumber = 1,
            PageSize = 10,
            SortColumnName = "Name",
            SortOrder = SortOrder.Desc
        };


        // Act
        var (lisResponse, result) = await Fixture.PlatformAdmin.GETAsync<ListEndpoint, ListRequest, PagedResponse<CSOModel>>(listRequest);

        // Assert
        lisResponse.IsSuccessStatusCode.Should().BeTrue();
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(22);
        result.Items.Count.Should().Be(10);
        // should be  D19,  D18, D17, D16, D15

        result.Items[0].Name.Should().Be($"Deactivated19");
        result.Items[1].Name.Should().Be($"Deactivated18");
        result.Items[2].Name.Should().Be($"Deactivated17");
        result.Items[3].Name.Should().Be($"Deactivated16");
        result.Items[4].Name.Should().Be($"Deactivated15");


    }

    [Fact]
    public async Task Should_ListCso_WhenValidRequestDataFiltering_Status_Activated_Sorting_Name_Desc_PageSize10Page1()
    {

        // Arrange
        await Fixture.PlatformAdmin.GenerateCSOTestData(Fixture.OutputHelper!);

        var listRequest = new ListRequest
        {
            NameFilter = null,
            Status = CSOStatus.Activated,
            PageNumber = 1,
            PageSize = 10,
            SortColumnName = "Name",
            SortOrder = SortOrder.Desc
        };


        // Act
        var (lisResponse, result) = await Fixture.PlatformAdmin.GETAsync<ListEndpoint, ListRequest, PagedResponse<CSOModel>>(listRequest);

        // Assert
        lisResponse.IsSuccessStatusCode.Should().BeTrue();
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(20);
        result.Items.Count.Should().Be(10);
        // should be  A9,  A8, A7, A6, A5

        result.Items[0].Name.Should().Be($"Activated9");
        result.Items[1].Name.Should().Be($"Activated8");
        result.Items[2].Name.Should().Be($"Activated7");
        result.Items[3].Name.Should().Be($"Activated6");
        result.Items[4].Name.Should().Be($"Activated5");
    }

    [Fact]
    public async Task Should_ListCso_WhenValidRequestDataFiltering_Status_Asc_Sorting_Name_ASC_PageSize10Page1()
    {

        // Arrange
        await Fixture.PlatformAdmin.GenerateCSOTestData(Fixture.OutputHelper!);

        var listRequest = new ListRequest
        {
            NameFilter = null,
            Status = CSOStatus.Activated,
            PageNumber = 1,
            PageSize = 10,
            SortColumnName = "Name",
            SortOrder = SortOrder.Asc
        };


        // Act
        var (lisResponse, result) = await Fixture.PlatformAdmin.GETAsync<ListEndpoint, ListRequest, PagedResponse<CSOModel>>(listRequest);

        // Assert
        lisResponse.IsSuccessStatusCode.Should().BeTrue();
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(20);
        result.Items.Count.Should().Be(10);
        // should be  A0, A1,  A10, A11, A12, A13

        result.Items[0].Name.Should().Be($"Activated0");
        result.Items[1].Name.Should().Be($"Activated1");
        result.Items[2].Name.Should().Be($"Activated10");
        result.Items[3].Name.Should().Be($"Activated11");
        result.Items[4].Name.Should().Be($"Activated12");
        result.Items[5].Name.Should().Be($"Activated13");
    }
}
