namespace Vote.Monitor.Api.Feature.Observer.UnitTests.Services;
public class CsvRowParsedHelpersTests
{

    [Fact]
    public void ConstructErrorFileContent_WithSuccessfulItems_ReturnsCorrectContent()
    {
        // Arrange
        var items = new List<CsvRowParsed<string>>();
        items.Add(new CsvRowParsed<string> { OriginalRow = "SuccessRow1", IsSuccess = true, ErrorMessage = null });
        items.Add(new CsvRowParsed<string> { OriginalRow = "SuccessRow2", IsSuccess = true, ErrorMessage = null });

        // Act
        var result = items.ConstructErrorFileContent();

        // Assert
        Assert.Equal($"SuccessRow1{Environment.NewLine}SuccessRow2{Environment.NewLine}", result);
    }

    [Fact]
    public void ConstructErrorFileContent_WithFailedItems_ReturnsCorrectContent()
    {
        // Arrange
        var items = new List<CsvRowParsed<string>>();
        items.Add(new CsvRowParsed<string> { OriginalRow = "ErrorRow1", IsSuccess = false, ErrorMessage = "Error1" });
        items.Add(new CsvRowParsed<string> { OriginalRow = "ErrorRow2", IsSuccess = false, ErrorMessage = "Error2" });

        // Act
        var result = items.ConstructErrorFileContent();

        // Assert
        Assert.Equal($"ErrorRow1,Error1{Environment.NewLine}ErrorRow2,Error2{Environment.NewLine}", result);
    }

    [Fact]
    public void ConstructErrorFileContent_WithMixedItems_ReturnsCorrectContent()
    {
        // Arrange
        var items = new List<CsvRowParsed<string>>();
        items.Add(new CsvRowParsed<string> { OriginalRow = "SuccessRow1", IsSuccess = true, ErrorMessage = null });
        items.Add(new CsvRowParsed<string> { OriginalRow = "ErrorRow1", IsSuccess = false, ErrorMessage = "Error1" });
        items.Add(new CsvRowParsed<string> { OriginalRow = "SuccessRow2", IsSuccess = true, ErrorMessage = null });

        // Act
        var result = items.ConstructErrorFileContent();

        // Assert
        Assert.Equal($"SuccessRow1{Environment.NewLine}ErrorRow1,Error1{Environment.NewLine}SuccessRow2{Environment.NewLine}", result);
    }
    [Fact]
    public void CheckAndSetDuplicatesLines_WithoutDuplicates_ReturnsFalse()
    {
        // Arrange
        var rows = new List<CsvRowParsed<string>>
        {
            new CsvRowParsed<string> {OriginalRow = "email1",  IsSuccess = true, Value = "email1" },
            new CsvRowParsed<string> { OriginalRow = "email2",IsSuccess = true, Value = "email2" },
            new CsvRowParsed<string> { OriginalRow = "email3", IsSuccess = true, Value = "email3" }
        };

        // Act
        var result = rows.CheckAndSetDuplicatesLines();

        // Assert
        Assert.False(result);
        Assert.All(rows, row =>
        {
            Assert.True(row.IsSuccess);
            Assert.Null(row.ErrorMessage);
        });
    }

    [Fact]
    public void CheckAndSetDuplicatesLines_WithoutDuplicatesContainNullValues_ReturnsFalse()
    {
        // Arrange
        var rows = new List<CsvRowParsed<string>>
        {
            new CsvRowParsed<string> {OriginalRow = "email",  IsSuccess = true, Value = null },
            new CsvRowParsed<string> { OriginalRow = "email2",IsSuccess = true, Value = "email2" },
            new CsvRowParsed<string> { OriginalRow = "email3", IsSuccess = true, Value = "email3" }
        };

        // Act
        var result = rows.CheckAndSetDuplicatesLines();

        // Assert
        Assert.False(result);
        Assert.All(rows, row =>
        {
            Assert.True(row.IsSuccess);
            Assert.Null(row.ErrorMessage);
        });
    }

    [Fact]
    public void CheckAndSetDuplicatesLines_With1Duplicates_SetsErrorMessagesAndReturnsTrue()
    {
        // Arrange
        var rows = new List<CsvRowParsed<string>>
        {
            new CsvRowParsed<string> { OriginalRow = "x", IsSuccess = true, Value = "email1" },
            new CsvRowParsed<string> {  OriginalRow = "x",IsSuccess = true, Value = "email2" },
            new CsvRowParsed<string> { OriginalRow = "x",  IsSuccess = true, Value = "email1" },
            new CsvRowParsed<string> {OriginalRow = "email1",  IsSuccess = true, Value = "email3" }
        };

        // Act
        var result = rows.CheckAndSetDuplicatesLines();

        // Assert
        Assert.True(result);
        Assert.Equal("Duplicated data found. First row where you can find the duplicate data is 1", rows[2].ErrorMessage);
        Assert.Equal("Duplicated data found. Row(s) where you can find the duplicate data are 3", rows[0].ErrorMessage);

        Assert.All(rows, row =>
        {
            if (row.Value == "email1")
            {
                Assert.False(row.IsSuccess);
            }
            else
            {
                Assert.True(row.IsSuccess);
                Assert.Null(row.ErrorMessage);
            }
        });
    }
    [Fact]
    public void CheckAndSetDuplicatesLines_With2DuplicatesForSameValue_SetsErrorMessagesAndReturnsTrue()
    {
        // Arrange
        var rows = new List<CsvRowParsed<string>>
        {
            new CsvRowParsed<string> { OriginalRow = "x", IsSuccess = true, Value = "email1" },
            new CsvRowParsed<string> {  OriginalRow = "x",IsSuccess = true, Value = "email2" },
            new CsvRowParsed<string> { OriginalRow = "x",  IsSuccess = true, Value = "email1" },
            new CsvRowParsed<string> {OriginalRow = "email1",  IsSuccess = true, Value = "email3" },
            new CsvRowParsed<string> { OriginalRow = "33x", IsSuccess = true, Value = "email1" }
        };

        // Act
        var result = rows.CheckAndSetDuplicatesLines();

        // Assert
        Assert.True(result);
        Assert.Equal("Duplicated data found. First row where you can find the duplicate data is 1", rows[2].ErrorMessage);
        Assert.Equal("Duplicated data found. First row where you can find the duplicate data is 1", rows[4].ErrorMessage);
        Assert.Equal("Duplicated data found. Row(s) where you can find the duplicate data are 3, 5", rows[0].ErrorMessage);

        Assert.All(rows, row =>
        {
            if (row.Value == "email1")
            {
                Assert.False(row.IsSuccess);
            }
            else
            {
                Assert.True(row.IsSuccess);
                Assert.Null(row.ErrorMessage);
            }
        });
    }

}
