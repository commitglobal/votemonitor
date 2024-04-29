using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.TestUtils.Utils;

namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public class RatingQuestionTests
{
    [Fact]
    public void ComparingToARatingQuestion_WithSameProperties_ReturnsTrue()
    {
        // Arrange
        var text = new TranslatedString
        {
            {"EN", "some text"}
        };

        var helptext = new TranslatedString
        {
            {"EN", "other text"}
        };

        var id = Guid.NewGuid();
        var ratingQuestion1 = RatingQuestion.Create(id, "C!", text, RatingScale.OneTo10, helptext);
        var ratingQuestion2 = ratingQuestion1.DeepClone();

        // Act
        var result = ratingQuestion1 == ratingQuestion2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingToARatingQuestion_WithDifferentProperties_ReturnsFalse()
    {
        // Arrange
        var text1 = new TranslatedString
        {
            {"EN", "some text"}
        };

        var text2 = new TranslatedString
        {
            {"EN", "some text"}
        };

        var helptext1 = new TranslatedString
        {
            {"EN", "other text"}
        };

        var helptext2 = new TranslatedString
        {
            {"EN", "other different"}
        };

        var id = Guid.NewGuid();

        var ratingQuestion1 = RatingQuestion.Create(id, "C!", text1, RatingScale.OneTo10, helptext1);
        var ratingQuestion2 = RatingQuestion.Create(id, "C!", text2, RatingScale.OneTo10, helptext2);

        // Act
        var result = ratingQuestion1 == ratingQuestion2;

        // Assert
        result.Should().BeFalse();
    }
}
