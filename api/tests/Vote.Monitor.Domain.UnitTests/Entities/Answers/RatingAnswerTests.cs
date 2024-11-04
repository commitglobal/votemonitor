using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Domain.UnitTests.Entities.Answers;

public class RatingAnswerTests
{
    [Fact]
    public void ComparingToARatingAnswer_WithSameProperties_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var answer1 = RatingAnswer.Create(id, 3);
        var answer2 = answer1.DeepClone();

        // Act
        var result = answer1 == answer2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingToARatingAnswer_WithDifferentProperties_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var answer1 = RatingAnswer.Create(id, 2);
        var answer2 = RatingAnswer.Create(id, 3);

        // Act
        var result = answer1 == answer2;

        // Assert
        result.Should().BeFalse();
    }
}
