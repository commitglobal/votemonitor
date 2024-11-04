using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Domain.UnitTests.Entities.Answers;

public record SingleSelectAnswerTests
{
    [Fact]
    public void ComparingToASingleSelectAnswer_WithSameProperties_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var answer1 = SingleSelectAnswer.Create(id, SelectedOption.Create(Guid.NewGuid(), "some text"));
        var answer2 = answer1.DeepClone();

        // Act
        var result = answer1 == answer2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingToASingleSelectAnswer_WithDifferentProperties_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var optionId = Guid.NewGuid();
        var answer1 = SingleSelectAnswer.Create(id, SelectedOption.Create(optionId, "some text"));
        var answer2 = SingleSelectAnswer.Create(id, SelectedOption.Create(optionId, "some other text"));


        // Act
        var result = answer1 == answer2;

        // Assert
        result.Should().BeFalse();
    }
}
