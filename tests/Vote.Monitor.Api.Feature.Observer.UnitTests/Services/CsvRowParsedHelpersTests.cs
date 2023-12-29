using Vote.Monitor.Api.Feature.Observer.Services;

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
        Assert.Equal("SuccessRow1\nSuccessRow2\n", result);
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
        Assert.Equal("ErrorRow1,Error1\nErrorRow2,Error2\n", result);
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
        Assert.Equal("SuccessRow1\nErrorRow1,Error1\nSuccessRow2\n", result);
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
        Assert.Equal("Duplicated email found. First row where you can find the duplicate email is 0", rows[2].ErrorMessage);
        Assert.Equal("Duplicated email found. Row(s) where you can find the duplicate email are 2", rows[0].ErrorMessage);

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
        Assert.Equal("Duplicated email found. First row where you can find the duplicate email is 0", rows[2].ErrorMessage);
        Assert.Equal("Duplicated email found. First row where you can find the duplicate email is 0", rows[4].ErrorMessage);
        Assert.Equal("Duplicated email found. Row(s) where you can find the duplicate email are 2, 4", rows[0].ErrorMessage);

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
