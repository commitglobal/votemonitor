using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;
using Vote.Monitor.TestUtils.Utils;

namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public class MultiSelectQuestionTests
{
    [Fact]
    public void ComparingToAMultiSelectQuestion_WithSameProperties_ReturnsTrue()
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

        SelectOption[] options = [.. new SelectOptionFaker().Generate(3)];

        var id = Guid.NewGuid();
        var multiSelectQuestion1 = MultiSelectQuestion.Create(id, "C!", text, helptext, options);
        var multiSelectQuestion2 = multiSelectQuestion1.DeepClone();

        // Act
        var result = multiSelectQuestion1 == multiSelectQuestion2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingToAMultiSelectQuestion_WithDifferentProperties_ReturnsFalse()
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

        SelectOption[] options1 = [.. new SelectOptionFaker().Generate(3)];
        SelectOption[] options2 = [.. new SelectOptionFaker().Generate(3)];

        var id = Guid.NewGuid();

        var textQuestion1 = MultiSelectQuestion.Create(id, "C!", text1, helptext1, options1);
        var textQuestion2 = MultiSelectQuestion.Create(id, "C!", text2, helptext2, options2);

        // Act
        var result = textQuestion1 == textQuestion2;

        // Assert
        result.Should().BeFalse();
    }
}
