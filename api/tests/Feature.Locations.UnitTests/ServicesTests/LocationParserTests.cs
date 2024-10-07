using Vote.Monitor.Core.Models;
using Vote.Monitor.TestUtils.Utils;

namespace Feature.Locations.UnitTests.ServicesTests;

public class LocationParserTests
{
    private readonly IOptions<LocationParserConfig> _parserConfig = new LocationParserConfig
    {
        MaxParserErrorsReturned = 10
    }.AsIOption();

    private readonly Faker _faker = new();

    [Fact]
    public void Parsing_ShouldFail_When_Invalid_Data()
    {
        // Arrange
        var reader = Substitute.For<ICsvReader<LocationImportModel>>();
        var parsedRows = new LocationImportModel().Repeat(1);

        reader
            .Read<LocationImportModelMapper>(Arg.Any<Stream>())
            .Returns(parsedRows);

        var sut = new LocationParser(reader, NullLogger<LocationParser>.Instance, _parserConfig);

        // Act
        var result = sut.Parse(new MemoryStream());

        // Assert
        result.Should().BeOfType<LocationParsingResult.Fail>()
            .Which.ValidationErrors
            .Should().HaveCount(1)
            .And
            .Subject.First().Errors.Should().HaveCount(1);
    }

    [Fact]
    public void Parsing_ShouldFail_AndRespectMaxErrorConfig_When_Invalid_Data()
    {
        // Arrange
        var reader = Substitute.For<ICsvReader<LocationImportModel>>();
        var parsedRows = new LocationImportModel().Repeat(121);
        reader
            .Read<LocationImportModelMapper>(Arg.Any<Stream>())
            .Returns(parsedRows);

        var sut = new LocationParser(reader, NullLogger<LocationParser>.Instance, _parserConfig);

        // Act
        var result = sut.Parse(new MemoryStream());

        // Assert
        result.Should().BeOfType<LocationParsingResult.Fail>()
            .Which.ValidationErrors
            .Should().HaveCount(10);
    }

    [Fact]
    public void Parsing_ShouldSucceed_When_ValidData()
    {
        // Arrange
        var reader = Substitute.For<ICsvReader<LocationImportModel>>();
        var parsedRows = Enumerable.Range(1, 100)
            .Select(_ => new LocationImportModel
            {
                Level1 = "Level1",
                Level2 = "Level2",
                Level3 = "Level3",
                Level4 = "Level4",
                Level5 = "Level5",
                DisplayOrder = _faker.Random.Int(0, 100),
                Tags = new List<TagImportModel>
                {
                    new() { Name = "Tag1", Value = "value1" },
                    new() { Name = "Tag2", Value = "value2" }
                }
            }).ToList();

        reader
            .Read<LocationImportModelMapper>(Arg.Any<Stream>())
            .Returns(parsedRows);

        var sut = new LocationParser(reader, NullLogger<LocationParser>.Instance, _parserConfig);

        // Act
        var result = sut.Parse(new MemoryStream());

        // Assert
        result.Should().BeOfType<LocationParsingResult.Success>()
            .Which.Locations
            .Should().HaveCount(100)
            .And.Subject.Should().BeEquivalentTo(parsedRows);
    }

    [Fact]
    public void Parsing_ShouldSucceed_When_ValidCsv()
    {
        // Arrange
        var reader = new CsvReader<LocationImportModel>();
        var sut = new LocationParser(reader, NullLogger<LocationParser>.Instance, _parserConfig);

        var csvData = "Level1,Level2,Level3,Level4,Level5,DisplayOrder,Tag1,Tag2\n" +
                      "Level1,Level2,Level3,Level4,Level5,1,TagA,TagB\n" +
                      "Level1,Level2,Level3,Level4,Level5,2,TagC,TagD\n" +
                      "Level1,Level2,Level3,Level4,Level5,3,TagE,TagF";
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        writer.Write(csvData);
        writer.Flush();
        stream.Position = 0;

        // Act
        var result = sut.Parse(stream);

        // Assert
        result.Should().BeOfType<LocationParsingResult.Success>()
            .Which.Locations
            .Should().HaveCount(3);

        result.Should().BeOfType<LocationParsingResult.Success>()
            .Which.Locations[0]
            .Should().BeEquivalentTo(new LocationImportModel
            {
                Level1 = "Level1",
                Level2 = "Level2",
                Level3 = "Level3",
                Level4 = "Level4",
                Level5 = "Level5",
                DisplayOrder = 1,
                Tags = new List<TagImportModel>
                {
                    new()
                    {
                        Name = "Tag1",
                        Value = "TagA"
                    },
                    new()
                    {
                        Name = "Tag2",
                        Value = "TagB"
                    }
                }
            });

        result.Should().BeOfType<LocationParsingResult.Success>()
            .Which.Locations[1]
            .Should().BeEquivalentTo(new LocationImportModel
            {
                Level1 = "Level1",
                Level2 = "Level2",
                Level3 = "Level3",
                Level4 = "Level4",
                Level5 = "Level5",
                DisplayOrder = 2,
                Tags = new List<TagImportModel>
                {
                    new()
                    {
                        Name = "Tag1",
                        Value = "TagC"
                    },
                    new()
                    {
                        Name = "Tag2",
                        Value = "TagD"
                    }
                }
            });

        result.Should().BeOfType<LocationParsingResult.Success>()
            .Which.Locations[2]
            .Should().BeEquivalentTo(new LocationImportModel
            {
                Level1 = "Level1",
                Level2 = "Level2",
                Level3 = "Level3",
                Level4 = "Level4",
                Level5 = "Level5",
                DisplayOrder = 3,
                Tags = new List<TagImportModel>
                {
                    new()
                    {
                        Name = "Tag1",
                        Value = "TagE"
                    },
                    new()
                    {
                        Name = "Tag2",
                        Value = "TagF"
                    }
                }
            });
    }
}