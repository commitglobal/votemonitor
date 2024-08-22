using Vote.Monitor.TestUtils.Utils;

namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class SingleSelectQuestionTests
{
    [Fact]
    public void ComparingToASingleSelectQuestion_WithSameProperties_ReturnsTrue()
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
        var singleSelectQuestion1 = SingleSelectQuestion.Create(id, "C!", text, options, helptext);
        var singleSelectQuestion2 = singleSelectQuestion1.DeepClone();

        // Act
        var result = singleSelectQuestion1 == singleSelectQuestion2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingToASingleSelectQuestion_WithDifferentProperties_ReturnsFalse()
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

        var textQuestion1 = SingleSelectQuestion.Create(id, "C!", text1, options1, helptext1);
        var textQuestion2 = SingleSelectQuestion.Create(id, "C!", text2, options2, helptext2);

        // Act
        var result = textQuestion1 == textQuestion2;

        // Assert
        result.Should().BeFalse();
    }
}
