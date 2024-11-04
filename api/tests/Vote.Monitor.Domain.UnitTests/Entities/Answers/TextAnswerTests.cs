using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Domain.UnitTests.Entities.Answers;

public record TextAnswerTests
{
    [Fact]
    public void ComparingToATextAnswer_WithSameProperties_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var answer1 = TextAnswer.Create(id, "Text");
        var answer2 = answer1.DeepClone();

        // Act
        var result = answer1 == answer2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingToATextAnswer_WithDifferentProperties_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var answer1 = TextAnswer.Create(id, "Text");
        var answer2 = TextAnswer.Create(id, "Other text");

        // Act
        var result = answer1 == answer2;

        // Assert
        result.Should().BeFalse();
    }
}
