using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Domain.UnitTests.Entities.Answers;

public class NumberAnswerTests
{
    [Fact]
    public void ComparingToANumberAnswer_WithSameProperties_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var answer1 = NumberAnswer.Create(id, 420);
        var answer2 = answer1.DeepClone();

        // Act
        var result = answer1 == answer2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingToANumberAnswer_WithDifferentProperties_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var answer1 = NumberAnswer.Create(id, 420);
        var answer2 = NumberAnswer.Create(id, 69);

        // Act
        var result = answer1 == answer2;

        // Assert
        result.Should().BeFalse();
    }
}
