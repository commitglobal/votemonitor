using System.Text.Json;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Hangfire.Jobs.Export.PollingStations;
using Vote.Monitor.Hangfire.Jobs.Export.PollingStations.ReadModels;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData;

public class PollingStationsDataTableGeneratorTests
{
    private readonly Guid _pollingStationId1 = Guid.NewGuid();
    private readonly Guid _pollingStationId2 = Guid.NewGuid();
    private readonly Guid _pollingStationId3 = Guid.NewGuid();

    private readonly string _pollingStationId1Tags = """
    {
        "tag1": "tag1_value1",
        "tag2": "tag2_value1",
        "tag4": "tag4_value1",
        "tag5": "tag5_value1"
    }
    """;
    private readonly string _pollingStationId2Tags = """
    {
        "tag1": "tag1_value2",
        "tag2": "tag2_value2",
        "tag3": "tag3_value2",
        "tag6": "tag6_value2"
    }

    """;
    private readonly string _pollingStationId3Tags = """
    {
        "tag1": "tag1_value3",
        "tag3": "tag3_value3",
        "tag4": "tag4_value3",
        "tag7": "tag7_value3"
    }
    """;

    private static readonly string[] _pollingStationColumns =
    [
        "Id",
        "Level1",
        "Level2",
        "Level3",
        "Level4",
        "Level5",
        "Number",
        "Address",
        "DisplayOrder",
    ];

    [Fact]
    public void PollingStationsDataTableGenerator_Should_Generate_DataTable_With_Default_Columns()
    {
        // Arrange
        var generator = PollingStationsDataTable
            .New()
            .WithData();

        // Act
        var result = generator.Please();

        // Assert
        result.Should().NotBeNull();
        result.header.Should().ContainInOrder(_pollingStationColumns);
        result.header.Should().HaveSameCount(_pollingStationColumns);
    }

    [Fact]
    public void PollingStationsDataTableGenerator_Should_Generates_Correct_DataTable_WhenEmptyTags()
    {
        // Arrange
        var pollingStation1 = Fake.PollingStation(_pollingStationId1);
        var pollingStation2 = Fake.PollingStation(_pollingStationId2);
        var pollingStation3 = Fake.PollingStation(_pollingStationId3);

        List<object[]> expectedData =
        [
            [
            .. GetDefaultExpectedColumns(pollingStation1),
            ],
            [
            .. GetDefaultExpectedColumns(pollingStation2),
            ],
            [
            .. GetDefaultExpectedColumns(pollingStation3),
            ],
        ];

        // Act
        var result = PollingStationsDataTable
            .New()
            .WithData()
            .For(pollingStation1)
            .For(pollingStation2)
            .For(pollingStation3)
            .Please();

        // Assert
        string[] expectedColumns = [.. _pollingStationColumns];

        result.Should().NotBeNull();
        result.header.Should().HaveSameCount(expectedColumns);
        result.header.Should().ContainInOrder(expectedColumns);
        result.dataTable.Should().HaveCount(3);

        result.dataTable[0].Should().ContainInOrder(expectedData[0]);
        result.dataTable[1].Should().ContainInOrder(expectedData[1]);
        result.dataTable[2].Should().ContainInOrder(expectedData[2]);
    }

    [Fact]
    public void PollingStationsDataTableGenerator_Should_Generates_Correct_DataTableWhenTags()
    {
        // Arrange
        var pollingStation1 = Fake.PollingStation(_pollingStationId1, JsonDocument.Parse(_pollingStationId1Tags));
        var pollingStation2 = Fake.PollingStation(_pollingStationId2, JsonDocument.Parse(_pollingStationId2Tags));
        var pollingStation3 = Fake.PollingStation(_pollingStationId3, JsonDocument.Parse(_pollingStationId3Tags));

        List<object[]> expectedData =
        [
            [
            .. GetDefaultExpectedColumns(pollingStation1),
            "tag1_value1",
            "tag2_value1",
            "",
            "tag4_value1",
            "tag5_value1",
            "",
            "",
            ],
            [
            .. GetDefaultExpectedColumns(pollingStation2),
            "tag1_value2",
            "tag2_value2",
            "tag3_value2",
            "",
            "",
            "tag6_value2",
            "",
            ],
            [
            .. GetDefaultExpectedColumns(pollingStation3),
            "tag1_value3",
            "",
            "tag3_value3",
            "tag4_value3",
            "",
            "",
            "tag7_value3",
            ],
        ];

        // Act
        var result = PollingStationsDataTable
            .New()
            .WithData()
            .For(pollingStation1)
            .For(pollingStation2)
            .For(pollingStation3)
            .Please();

        // Assert
        string[] expectedColumns = [
            .. _pollingStationColumns,
            "tag1",
            "tag2",
            "tag3",
            "tag4",
            "tag5",
            "tag6",
            "tag7",
        ];

        result.Should().NotBeNull();
        result.header.Should().HaveSameCount(expectedColumns);
        result.header.Should().ContainInOrder(expectedColumns);
        result.dataTable.Should().HaveCount(3);

        result.dataTable[0].Should().ContainInOrder(expectedData[0]);
        result.dataTable[1].Should().ContainInOrder(expectedData[1]);
        result.dataTable[2].Should().ContainInOrder(expectedData[2]);
    }

    private object[] GetDefaultExpectedColumns(PollingStationModel pollingStation)
    {
        return
        [
            pollingStation.Id.ToString(),
            pollingStation.Level1,
            pollingStation.Level2,
            pollingStation.Level3,
            pollingStation.Level4,
            pollingStation.Level5,
            pollingStation.Number,
            pollingStation.Address,
            pollingStation.DisplayOrder
        ];
    }
}
