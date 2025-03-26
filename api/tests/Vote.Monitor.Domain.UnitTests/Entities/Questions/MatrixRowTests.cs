using Vote.Monitor.Core.Helpers;

namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public class MatrixRowTests
{
    [Fact]
    public void ComparingToAMatrixRow_WithSameProperties_ReturnsTrue()
    {
        // Arrange
        var text = new TranslatedString
        {
            {"EN", "text"}
        };

        var id = Guid.NewGuid();
        var row1 = MatrixRow.Create(id, text);
        var row2 = row1.DeepClone();

        // Act
        var result = row1 == row2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingToAMatrixRow_WithDifferentProperties_ReturnsFalse()
    {
        // Arrange
        var text1 = new TranslatedString
        {
            {"EN", "text"}
        };

        var text2 = new TranslatedString
        {
            {"EN", "other tex"}
        };

        var id = Guid.NewGuid();
        var row1 = MatrixRow.Create(id, text1);
        var row2 = MatrixRow.Create(id, text2);

        // Act
        var result = row1 == row2;

        // Assert
        result.Should().BeFalse();
    }
}
