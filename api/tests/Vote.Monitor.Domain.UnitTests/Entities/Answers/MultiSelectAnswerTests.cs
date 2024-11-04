using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

namespace Vote.Monitor.Domain.UnitTests.Entities.Answers;

public class MultiSelectAnswerTests
{
    [Fact]
    public void ComparingToAMultiSelectAnswer_WithSameProperties_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var answer1 = MultiSelectAnswer.Create(id, new SelectedOptionFaker().Generate(3));
        var answer2 = answer1.DeepClone();

        // Act
        var result = answer1 == answer2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingToAMultiSelectAnswer_WithDifferentProperties_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var answer1 = MultiSelectAnswer.Create(id, new SelectedOptionFaker().Generate(3));
        var answer2 = MultiSelectAnswer.Create(id, new SelectedOptionFaker().Generate(3));

        // Act
        var result = answer1 == answer2;

        // Assert
        result.Should().BeFalse();
    }
}
