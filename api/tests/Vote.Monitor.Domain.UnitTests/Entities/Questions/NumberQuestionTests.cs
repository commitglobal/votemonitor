using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.TestUtils.Utils;

namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public class NumberQuestionTests
{
    [Fact]
    public void ComparingToANumberQuestion_WithSameProperties_ReturnsTrue()
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

        var inputPlaceholder = new TranslatedString
        {
            {"EN", "placeholder"}
        };

        var id = Guid.NewGuid();
        var numberQuestion1 = NumberQuestion.Create(id, "C!", text, helptext, inputPlaceholder);
        var numberQuestion2 = numberQuestion1.DeepClone();

        // Act
        var result = numberQuestion1 == numberQuestion2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingToANumberQuestion_WithDifferentProperties_ReturnsFalse()
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

        var inputPlaceholder1 = new TranslatedString
        {
            {"EN", "placeholder"}
        };

        var inputPlaceholder2 = new TranslatedString
        {
            {"EN", "placeholder"}
        };

        var id = Guid.NewGuid();

        var numberQuestion1 = NumberQuestion.Create(id, "C!", text1, helptext1, inputPlaceholder1);
        var numberQuestion2 = NumberQuestion.Create(id, "C!", text2, helptext2, inputPlaceholder2);

        // Act
        var result = numberQuestion1 == numberQuestion2;

        // Assert
        result.Should().BeFalse();
    }
}
