using Vote.Monitor.TestUtils.Utils;

namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class TextQuestionTests
{
    [Fact]
    public void ComparingToATextQuestion_WithSameProperties_ReturnsTrue()
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
        var textQuestion1 = TextQuestion.Create(id, "C!", text, helptext, inputPlaceholder);
        var textQuestion2 = textQuestion1.DeepClone();

        // Act
        var result = textQuestion1 == textQuestion2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingToATextQuestion_WithDifferentProperties_ReturnsFalse()
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

        var textQuestion1 = TextQuestion.Create(id, "C!", text1, helptext1, inputPlaceholder1);
        var textQuestion2 = TextQuestion.Create(id, "C!", text2, helptext2, inputPlaceholder2);

        // Act
        var result = textQuestion1 == textQuestion2;

        // Assert
        result.Should().BeFalse();
    }
}
