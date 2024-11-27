using Vote.Monitor.Api.Feature.Observer.Parser;
using Vote.Monitor.Core.Services.Parser;

namespace Vote.Monitor.Api.Feature.Observer.UnitTests.Services;
public class ObserverCsvParserTests
{
    [Fact]
    public void Parsing_ShouldFail_When_EmptyFile()
    {
        // Arrange
        var parser = new CsvParser<ObserverImportModel, ObserverImportModelMapper>(
                NullLogger<CsvParser<ObserverImportModel, ObserverImportModelMapper>>.Instance,
                new ObserverImportModelValidator());

        // Act
        var result = parser.Parse(new MemoryStream());

        // Assert
        result.Should().BeOfType<ParsingResult<ObserverImportModel>.Fail>();
        var failItemResult = result.As<ParsingResult<ObserverImportModel>.Fail>().Items;
        failItemResult.Should().HaveCount(1);
        failItemResult.First().ErrorMessage.Should().Be("Cannot parse the file or file empty.");

    }

    [Fact]
    public void Parsing_ShouldFail_When_Malformed_Header()
    {
        // Arrange
        string fileContent = "," + Environment.NewLine +
            "Obs1,obs1 @mail.com,2000000000" + Environment.NewLine +
            "Obs2,obs2 @mail.com,3000000000";
        using var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        var logger = Substitute.For<ILogger<CsvParser<ObserverImportModel, ObserverImportModelMapper>>>();

        var parser = new CsvParser<ObserverImportModel, ObserverImportModelMapper>(
                logger,
                new ObserverImportModelValidator());

        // Act
        var result = parser.Parse(fileStream);

        // Assert
        result.Should().BeOfType<ParsingResult<ObserverImportModel>.Fail>();
        var failItemResult = result.As<ParsingResult<ObserverImportModel>.Fail>().Items;
        failItemResult.Should().HaveCount(1);
        failItemResult.First().ErrorMessage.Should().Be("Cannot parse the header!");
    }

    [Theory]
    [method: MemberData(nameof(MalformedCsv))]
    public void Parsing_ShouldFail_When_MalformedCsv(string fileContent, int numberOfRows, List<int> rowIndexWithErrors, List<int> rowOk)
    {
        // Arrange
        using var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        var parser = new CsvParser<ObserverImportModel, ObserverImportModelMapper>(
                NullLogger<CsvParser<ObserverImportModel, ObserverImportModelMapper>>.Instance,
                new ObserverImportModelValidator());

        // Act
        var result = parser.Parse(fileStream);

        // Assert
        result.Should().BeOfType<ParsingResult<ObserverImportModel>.Fail>();
        var failItemResult = result.As<ParsingResult<ObserverImportModel>.Fail>().Items.ToList();
        failItemResult.Should().HaveCount(numberOfRows);
        for (int i = 0; i < rowIndexWithErrors.Count; i++)
        {
            int rowIndex = rowIndexWithErrors[i];
            failItemResult[rowIndex].IsSuccess.Should().BeFalse();
        }

        for (int i = 0; i < rowOk.Count; i++)
        {
            int rowIndex = rowOk[i];
            failItemResult[rowIndex].IsSuccess.Should().BeTrue();
        }
    }


    [Theory]
    [MemberData(nameof(ValidCsv))]
    public void Parsing_ShouldSucceed_When_ValidCSVs(string fileContent, List<ObserverImportModel> rows)
    {
        // Arrange
        using var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        var parser = new CsvParser<ObserverImportModel, ObserverImportModelMapper>(
                NullLogger<CsvParser<ObserverImportModel, ObserverImportModelMapper>>.Instance,
                new ObserverImportModelValidator());

        // Act
        var result = parser.Parse(fileStream);

        // Assert
        result.Should().BeOfType<ParsingResult<ObserverImportModel>.Success>();
        var succesItemResult = result.As<ParsingResult<ObserverImportModel>.Success>().Items;
        succesItemResult.Should().HaveCount(rows.Count);
        succesItemResult.Should().BeEquivalentTo(rows);
    }

    public static IEnumerable<object[]> ValidCsv => new List<object[]>
    {
         new object[] {
             "FirstName,LastName,Email,PhoneNumber,Password"
            + Environment.NewLine + "Obs1,test,obs1@mail.com,2000000000,pa$$word"
            + Environment.NewLine + "Obs2,test,obs2@mail.com,3000000000,pa$$word",
             new List<ObserverImportModel>{
                 new() {FirstName = "Obs1",LastName="test",Password="pa$$word", Email = "obs1@mail.com", PhoneNumber = "2000000000" },
                 new() {FirstName = "Obs2",LastName="test",Password="pa$$word", Email = "obs2@mail.com", PhoneNumber = "3000000000" }
             }
         },
         new object[] { "FirstName,LastName,Email,PhoneNumber,Password", new List<ObserverImportModel>() },
         new object[] {  "FirstName,LastName,Email,PhoneNumber,Password"
                + Environment.NewLine + "Obs1,test,obs1@mail.com,2000000000,pa$$word",
             new List<ObserverImportModel>{
                 new() {FirstName = "Obs1", LastName="test",Password="pa$$word",Email = "obs1@mail.com", PhoneNumber = "2000000000" }
             }
         }
    };

    public static IEnumerable<object[]> MalformedCsv => new List<object[]>
    {
        // all rows are malformed
        new object[] {
            "FirstName,LastName,Email,PhoneNumber,Password"
            + Environment.NewLine + ",,obs1@mail.com,2000000000,"
            + Environment.NewLine + ",,obs2@mail.com,3000000000,",
            3,
            new List<int>{1,2},
            new List<int>()
        }, 
        //last row is malformed
        new object[]
        {
            "FirstName,LastName,Email,PhoneNumber,Password"
            + Environment.NewLine + "Obs1,Test,obs1@mail.com,2000000000,pa$$word"
            + Environment.NewLine + "Obs2,Test,",
            3,
            new List<int>{2},
            new List<int>{1}
        },
        //middle row is malformed
        new object[]
        {
            "FirstName,LastName,Email,PhoneNumber,Password"
            + Environment.NewLine + "Obs1,Test,obs1@mail.com,2000000000,pa$$word"
            + Environment.NewLine + "Obs5"
            + Environment.NewLine + "Obs2,Test,obs2@mail.com,2000000000,pa$$word"
            + Environment.NewLine + "Obs3,Test,obs3@mail.com,3000000000,pa$$word"
            + Environment.NewLine + "Obs4,Test,obs4@mail.com,4000000000,pa$$word",
            6,
            new List<int>{2},
            new List<int>{0,3,4,5}
        },
        //invalid observer data
        new object[]
        {
            "FirstName,LastName,Email,PhoneNumber,Password"
            + Environment.NewLine + "Observer1,Test,obs1@mail.com,2000000000,pa$$word"
            + Environment.NewLine + ",obs2@mail.com,3000000000,"
            + Environment.NewLine + "Observer3,Test,,4000000000,pa$$word"
            + Environment.NewLine + "Observer4,Test,obs4@mail.com,pa$$word",
            5,
            new List<int>{2,3,4},
            new List<int>{1}
        },
        //duplicate email
        new object[]
        {
            "FirstName,LastName,Email,PhoneNumber,Password" + Environment.NewLine +
            "Obs1,Test,obs1@mail.com,2000000000,pa$$word" + Environment.NewLine +
            "Obs2,Test,obs1@mail.com,3000000000,pa$$word",
            3,
            new List<int>{1,2},
            new List<int>()
        }
    };
}


