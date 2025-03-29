using Vote.Monitor.Core.Helpers;

namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public class MatrixOptionTests
{
    [Fact]
    public void ComparingToAMatrixOption_WithSameProperties_ReturnsTrue()
    {
        // Arrange
        var text = new TranslatedString
        {
            {"EN", "text"}
        };

        var id = Guid.NewGuid();
        var option1 = MatrixOption.Create(id, text, true);
        var option2 = option1.DeepClone();

        // Act
        var result = option1 == option2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingToAMatrixOption_WithDifferentProperties_ReturnsFalse()
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
        var option1 = MatrixOption.Create(id, text1, true);
        var option2 = MatrixOption.Create(id, text2, true);

        // Act
        var result = option1 == option2;

        // Assert
        result.Should().BeFalse();
    }
}
