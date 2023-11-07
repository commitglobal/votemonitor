namespace Vote.Monitor.Api.Feature.PollingStation.UnitTests.ServicesTests;

public class PollingStationParserTests
{
    private readonly IOptions<PollingStationParserConfig> _parserConfig = new PollingStationParserConfig
    {
        MaxParserErrorsReturned = 10
    }.AsIOption();

    private readonly Faker _faker = new();

    [Fact]
    public void Parsing_ShouldFail_When_Invalid_Data()
    {
        // Arrange
        var reader = Substitute.For<ICsvReader<PollingStationImportModel>>();
        var parsedRows = new PollingStationImportModel().Repeat(1);

        reader
            .Read<PollingStationImportModelMapper>(Arg.Any<Stream>())
            .Returns(parsedRows);

        var sut = new PollingStationParser(reader, NullLogger<PollingStationParser>.Instance, _parserConfig);

        // Act
        var result = sut.Parse(new MemoryStream());

        // Assert
        result.Should().BeOfType<PollingStationParsingResult.Fail>()
            .Which.ValidationErrors
            .Should().HaveCount(1)
            .And
            .Subject.First().Errors.Should().HaveCount(2);
    }

    [Fact]
    public void Parsing_ShouldFail_AndRespectMaxErrorConfig_When_Invalid_Data()
    {
        // Arrange
        var reader = Substitute.For<ICsvReader<PollingStationImportModel>>();
        var parsedRows = new PollingStationImportModel().Repeat(121);
        reader
            .Read<PollingStationImportModelMapper>(Arg.Any<Stream>())
            .Returns(parsedRows);

        var sut = new PollingStationParser(reader, NullLogger<PollingStationParser>.Instance, _parserConfig);

        // Act
        var result = sut.Parse(new MemoryStream());

        // Assert
        result.Should().BeOfType<PollingStationParsingResult.Fail>()
            .Which.ValidationErrors
            .Should().HaveCount(10);
    }

    [Fact]
    public void Parsing_ShouldSucceed_When_ValidData()
    {
        // Arrange
        var reader = Substitute.For<ICsvReader<PollingStationImportModel>>();
        var parsedRows = Enumerable.Range(1, 100)
            .Select(_ => new PollingStationImportModel
            {
                Address = _faker.Address.FullAddress(),
                DisplayOrder = _faker.Random.Int(0, 100),
                Tags = new List<TagImportModel>
                {
                    new() { Name = "Tag1", Value = "value1" },
                    new() { Name = "Tag2", Value = "value2" },
                },
            }).ToList();

        reader
            .Read<PollingStationImportModelMapper>(Arg.Any<Stream>())
            .Returns(parsedRows);

        var sut = new PollingStationParser(reader, NullLogger<PollingStationParser>.Instance, _parserConfig);

        // Act
        var result = sut.Parse(new MemoryStream());

        // Assert
        result.Should().BeOfType<PollingStationParsingResult.Success>()
            .Which.PollingStations
            .Should().HaveCount(100)
            .And.Subject.Should().BeEquivalentTo(parsedRows);
    }

    [Fact]
    public void Parsing_ShouldSucceed_When_ValidCsv()
    {
        // Arrange
        var reader = new CsvReader<PollingStationImportModel>();
        var sut = new PollingStationParser(reader, NullLogger<PollingStationParser>.Instance, _parserConfig);

        var csvData = "DisplayOrder,Address,Tag1,Tag2\n" +
                      "1,Address1,TagA,TagB\n" +
                      "2,Address2,TagC,TagD\n" +
                      "3,Address3,TagE,TagF";
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        writer.Write(csvData);
        writer.Flush();
        stream.Position = 0;

        // Act
        var result = sut.Parse(stream);

        // Assert
        result.Should().BeOfType<PollingStationParsingResult.Success>()
            .Which.PollingStations
            .Should().HaveCount(3);

        result.Should().BeOfType<PollingStationParsingResult.Success>()
            .Which.PollingStations[0]
            .Should().BeEquivalentTo(new PollingStationImportModel
            {
                DisplayOrder = 1,
                Address = "Address1",
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

        result.Should().BeOfType<PollingStationParsingResult.Success>()
            .Which.PollingStations[1]
            .Should().BeEquivalentTo(new PollingStationImportModel
            {
                DisplayOrder = 2,
                Address = "Address2",
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

        result.Should().BeOfType<PollingStationParsingResult.Success>()
            .Which.PollingStations[2]
            .Should().BeEquivalentTo(new PollingStationImportModel
            {
                DisplayOrder = 3,
                Address = "Address3",
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

    [Theory]
    [MemberData(nameof(MalformedCsvTestCases))]
    public void Parsing_ShouldFail_When_MalformedCsv(string malformedCsvData, string expectedErrorMessage)
    {
        // Arrange
        var reader = new CsvReader<PollingStationImportModel>();
        var sut = new PollingStationParser(reader, NullLogger<PollingStationParser>.Instance, _parserConfig);

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        writer.Write(malformedCsvData);
        writer.Flush();
        stream.Position = 0;

        // Act
        var result = sut.Parse(stream);

        // Assert
        result.Should().BeOfType<PollingStationParsingResult.Fail>();
        result.As<PollingStationParsingResult.Fail>().ValidationErrors[0].Errors[0].ErrorMessage.Should().Be(expectedErrorMessage);
    }

    public static IEnumerable<object[]> MalformedCsvTestCases =>
        new List<object[]>
        {
            // Extra tag name no tag values
            new object[] {
                ",\n" +
                           "1,Address1,TagA,TagB\n" +
                           "2,Address2,TagC,TagD\n" +
                           "3,Address3,TagE,TagF",
                "Invalid header provided in import polling stations file."
            },
            // Extra tag values no tag name
            new object[] {
                "DisplayOrder,Address,Tag1,Tag2\n" +
                           "1,Address1,TagATagB,\n" +
                           "2,Address2,TagC,TagD,\n" +
                           "3,",
                "Malformed import polling stations file provided."
            },
            // No display order
            new object[] {
                "DisplayOrder,Address,Tag1,Tag2\n" +
                           ",Address1,TagATagB,\n" +
                           ",Address2,TagC,TagD,\n" +
                           ",",
                "Malformed import polling stations file provided."
            }
        };
}
