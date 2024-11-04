using Vote.Monitor.Core.Helpers;

namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class TextQuestionTests
{
    private readonly string _defaultLanguageCode = LanguagesList.EN.Iso1;
    private readonly string _languageCode = LanguagesList.RO.Iso1;

    private TranslatedString CreateTranslatedString(string value)
    {
        return new TranslatedString
        {
            [_defaultLanguageCode] = "some text for default language",
            [_languageCode] = value
        };
    }
    
    [Fact]
    public void ComparingToATextQuestion_WithSameProperties_ReturnsTrue()
    {
        // Arrange
        var text = new TranslatedString
        {
            {_defaultLanguageCode, "some text"}
        };

        var helptext = new TranslatedString
        {
            {_defaultLanguageCode, "other text"}
        };

        var inputPlaceholder = new TranslatedString
        {
            {_defaultLanguageCode, "placeholder"}
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
            {_defaultLanguageCode, "some text"}
        };

        var text2 = new TranslatedString
        {
            {_defaultLanguageCode, "some text"}
        };

        var helptext1 = new TranslatedString
        {
            {_defaultLanguageCode, "other text"}
        };

        var helptext2 = new TranslatedString
        {
            {_defaultLanguageCode, "other different"}
        };

        var inputPlaceholder1 = new TranslatedString
        {
            {_defaultLanguageCode, "placeholder"}
        };

        var inputPlaceholder2 = new TranslatedString
        {
            {_defaultLanguageCode, "placeholder"}
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
