using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Domain.UnitTests.Entities.Answers;

public class DateAnswerTests
{
    [Fact]
    public void ComparingToADateAnswer_WithSameProperties_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var answer1 = DateAnswer.Create(id, DateTime.UtcNow);
        var answer2 = answer1.DeepClone();

        // Act
        var result = answer1 == answer2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingToADateAnswer_WithDifferentProperties_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var answer1 = DateAnswer.Create(id, DateTime.UtcNow);
        var answer2 = DateAnswer.Create(id, DateTime.UtcNow);

        // Act
        var result = answer1 == answer2;

        // Assert
        result.Should().BeFalse();
    }
}
