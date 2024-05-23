using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Hangfire.Jobs.Export.QuickReports;
using Vote.Monitor.Hangfire.Jobs.Export.QuickReports.ReadModels;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData;

public class QuickReportsDataTableGeneratorTests
{
    private const string Attachment1Url = "https://example.com/1";
    private const string Attachment2Url = "https://example.com/2";

    private readonly Guid _quickReportId1 = Guid.NewGuid();
    private readonly Guid _quickReportId2 = Guid.NewGuid();
    private readonly Guid _quickReportId3 = Guid.NewGuid();

    private static readonly string[] _quickReportColumns =
    [
        "QuickReportId",
        "TimeSubmitted",
        "FollowUpStatus",
        "MonitoringObserverId",
        "FirstName",
        "LastName",
        "Email",
        "PhoneNumber",
        "LocationType",
        "PollingStationId",
        "Level1",
        "Level2",
        "Level3",
        "Level4",
        "Level5",
        "LevelNumber",
        "PollingStationDetails",
        "Title",
        "Description",
    ];

    [Fact]
    public void QuickReportsDataTableGenerator_Should_Generate_DataTable_With_Default_Columns()
    {
        // Arrange
        var generator = QuickReportsDataTable
            .New()
            .WithData();

        // Act
        var result = generator.Please();

        // Assert
        result.Should().NotBeNull();
        result.header.Should().ContainInOrder(_quickReportColumns);
        result.header.Should().HaveSameCount(_quickReportColumns);
    }

    [Fact]
    public void QuickReportsDataTableGenerator_Should_Generates_Correct_DataTable()
    {
        // Arrange
        var quickReport1 = Fake.QuickReport(_quickReportId1, QuickReportLocationType.VisitedPollingStation, []);
        var quickReport2 = Fake.QuickReport(_quickReportId2, QuickReportLocationType.OtherPollingStation, GetOneAttachmentFor(_quickReportId2));
        var quickReport3 = Fake.QuickReport(_quickReportId3, QuickReportLocationType.NotRelatedToAPollingStation, GetTwoAttachmentsFor(_quickReportId3));

        List<object[]> expectedData =
        [
            [
                .. GetDefaultExpectedColumns(quickReport1),
                quickReport1.PollingStationId.ToString(),
                quickReport1.Level1,
                quickReport1.Level2,
                quickReport1.Level3,
                quickReport1.Level4,
                quickReport1.Level5,
                quickReport1.Number,
                "",
                quickReport1.Title,
                quickReport1.Description,
                "",
                ""
            ],
            [
                .. GetDefaultExpectedColumns(quickReport2),
                "",
                "",
                "",
                "",
                "",
                "",
                quickReport2.PollingStationDetails,
                quickReport2.Title,
                quickReport2.Description,
                Attachment1Url,
                ""
            ],
            [
                .. GetDefaultExpectedColumns(quickReport3),
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                quickReport3.Title,
                quickReport3.Description,
                Attachment1Url,
                Attachment2Url
            ],

        ];

        // Act
        var result = QuickReportsDataTable
            .New()
            .WithData()
            .For(quickReport1)
            .For(quickReport2)
            .For(quickReport3)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _quickReportColumns,
            "Attachment 1",
            "Attachment 2",
        ];

        result.Should().NotBeNull();
        result.header.Should().HaveSameCount(expectedColumns);
        result.header.Should().ContainInOrder(expectedColumns);
        result.dataTable.Should().HaveCount(3);

        result.dataTable[0].Should().ContainInOrder(expectedData[0]);
        result.dataTable[1].Should().ContainInOrder(expectedData[1]);
        result.dataTable[2].Should().ContainInOrder(expectedData[2]);
    }

    [Fact]
    public void QuickReportsDataTableGenerator_Should_Generates_Correct_DataTableWhenNoAttachments()
    {
        // Arrange
        var quickReport = Fake.QuickReport(_quickReportId1, QuickReportLocationType.VisitedPollingStation, []);
        List<object[]> expectedData =
        [
            [
                .. GetDefaultExpectedColumns(quickReport),
                quickReport.PollingStationId.ToString(),
                quickReport.Level1,
                quickReport.Level2,
                quickReport.Level3,
                quickReport.Level4,
                quickReport.Level5,
                quickReport.Number,
                "",
                quickReport.Title,
                quickReport.Description
            ],

        ];

        // Act
        var result = QuickReportsDataTable
            .New()
            .WithData()
            .For(quickReport)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _quickReportColumns,
        ];

        result.Should().NotBeNull();
        result.header.Should().HaveSameCount(expectedColumns);
        result.header.Should().ContainInOrder(expectedColumns);
        result.dataTable.Should().HaveCount(1);

        result.dataTable[0].Should().ContainInOrder(expectedData[0]);
    }

    private object[] GetDefaultExpectedColumns(QuickReportModel quickReport)
    {
        return
        [
            quickReport.Id.ToString(),
            quickReport.Timestamp.ToString("s"),
            quickReport.FollowUpStatus.Value,
            quickReport.MonitoringObserverId.ToString(),
            quickReport.FirstName,
            quickReport.LastName,
            quickReport.Email,
            quickReport.PhoneNumber,
            quickReport.QuickReportLocationType.Value
        ];
    }


    private QuickReportAttachmentModel[] GetOneAttachmentFor(Guid quickReportId)
    {
        return
        [
            new QuickReportAttachmentModel { QuickReportId = quickReportId, PresignedUrl = Attachment1Url },
        ];
    }
    private QuickReportAttachmentModel[] GetTwoAttachmentsFor(Guid quickReportId)
    {
        return
        [
            .. GetOneAttachmentFor(quickReportId),
            new QuickReportAttachmentModel { QuickReportId = quickReportId, PresignedUrl = Attachment2Url },
        ];
    }
}
