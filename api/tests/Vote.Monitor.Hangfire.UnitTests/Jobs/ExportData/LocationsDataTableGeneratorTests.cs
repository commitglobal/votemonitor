using System.Text.Json;
using Vote.Monitor.Hangfire.Jobs.Export.Locations;
using Vote.Monitor.Hangfire.Jobs.Export.Locations.ReadModels;
using Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData.Fakes;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData;

public class LocationsDataTableGeneratorTests
{
    private readonly Guid _locationId1 = Guid.NewGuid();
    private readonly Guid _locationId2 = Guid.NewGuid();
    private readonly Guid _locationId3 = Guid.NewGuid();

    private readonly string _locationId1Tags = """
                                               {
                                                   "tag1": "tag1_value1",
                                                   "tag2": "tag2_value1",
                                                   "tag4": "tag4_value1",
                                                   "tag5": "tag5_value1"
                                               }
                                               """;

    private readonly string _locationId2Tags = """
                                               {
                                                   "tag1": "tag1_value2",
                                                   "tag2": "tag2_value2",
                                                   "tag3": "tag3_value2",
                                                   "tag6": "tag6_value2"
                                               }

                                               """;

    private readonly string _locationId3Tags = """
                                               {
                                                   "tag1": "tag1_value3",
                                                   "tag3": "tag3_value3",
                                                   "tag4": "tag4_value3",
                                                   "tag7": "tag7_value3"
                                               }
                                               """;

    private static readonly string[] _locationColumns =
    [
        "Id",
        "Level1",
        "Level2",
        "Level3",
        "Level4",
        "Level5",
        "DisplayOrder",
    ];

    [Fact]
    public void LocationsDataTableGenerator_Should_Generate_DataTable_With_Default_Columns()
    {
        // Arrange
        var generator = LocationsDataTable
            .New()
            .WithData();

        // Act
        var result = generator.Please();

        // Assert
        result.Should().NotBeNull();
        result.header.Should().ContainInOrder(_locationColumns);
        result.header.Should().HaveSameCount(_locationColumns);
    }

    [Fact]
    public void LocationsDataTableGenerator_Should_Generates_Correct_DataTable_WhenEmptyTags()
    {
        // Arrange
        var location1 = Fake.Location(_locationId1);
        var location2 = Fake.Location(_locationId2);
        var location3 = Fake.Location(_locationId3);

        List<object[]> expectedData =
        [
            [
                .. GetDefaultExpectedColumns(location1),
            ],
            [
                .. GetDefaultExpectedColumns(location2),
            ],
            [
                .. GetDefaultExpectedColumns(location3),
            ],
        ];

        // Act
        var result = LocationsDataTable
            .New()
            .WithData()
            .For(location1)
            .For(location2)
            .For(location3)
            .Please();

        // Assert
        string[] expectedColumns = [.. _locationColumns];

        result.Should().NotBeNull();
        result.header.Should().HaveSameCount(expectedColumns);
        result.header.Should().ContainInOrder(expectedColumns);
        result.dataTable.Should().HaveCount(3);

        result.dataTable[0].Should().ContainInOrder(expectedData[0]);
        result.dataTable[1].Should().ContainInOrder(expectedData[1]);
        result.dataTable[2].Should().ContainInOrder(expectedData[2]);
    }

    [Fact]
    public void LocationsDataTableGenerator_Should_Generates_Correct_DataTableWhenTags()
    {
        // Arrange
        var location1 = Fake.Location(_locationId1, JsonDocument.Parse(_locationId1Tags));
        var location2 = Fake.Location(_locationId2, JsonDocument.Parse(_locationId2Tags));
        var location3 = Fake.Location(_locationId3, JsonDocument.Parse(_locationId3Tags));

        List<object[]> expectedData =
        [
            [
                .. GetDefaultExpectedColumns(location1),
                "tag1_value1",
                "tag2_value1",
                "",
                "tag4_value1",
                "tag5_value1",
                "",
                "",
            ],
            [
                .. GetDefaultExpectedColumns(location2),
                "tag1_value2",
                "tag2_value2",
                "tag3_value2",
                "",
                "",
                "tag6_value2",
                "",
            ],
            [
                .. GetDefaultExpectedColumns(location3),
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
        var result = LocationsDataTable
            .New()
            .WithData()
            .For(location1)
            .For(location2)
            .For(location3)
            .Please();

        // Assert
        string[] expectedColumns =
        [
            .. _locationColumns,
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

    private object[] GetDefaultExpectedColumns(LocationModel location)
    {
        return
        [
            location.Id.ToString(),
            location.Level1,
            location.Level2,
            location.Level3,
            location.Level4,
            location.Level5,
            location.DisplayOrder
        ];
    }
}