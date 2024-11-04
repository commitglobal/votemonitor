using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Domain.UnitTests.Entities.Answers;

public class SelectedOptionTests
{
    [Fact]
    public void ComparingToASelectedOption_WithSameProperties_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var option1 = SelectedOption.Create(id, "some text");
        var option2 = option1.DeepClone();

        // Act
        var result = option1 == option2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingToASelectedOption_WithDifferentProperties_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var option1 = SelectedOption.Create(id, "some text");
        var option2 = SelectedOption.Create(id, "some other text");

        // Act
        var result = option1 == option2;

        // Assert
        result.Should().BeFalse();
    }
}
