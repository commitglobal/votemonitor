using Vote.Monitor.Core.Helpers;

namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class DateQuestionTests
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
    public void ComparingToADateQuestion_WithSameProperties_ReturnsTrue()
    {
        // Arrange
        var text = new TranslatedString
        {
            { _defaultLanguageCode, "some text" }
        };

        var helptext = new TranslatedString
        {
            { _defaultLanguageCode, "other text" }
        };

        var id = Guid.NewGuid();
        var dateQuestion1 = DateQuestion.Create(id, "C!", text, helptext);
        var dateQuestion2 = dateQuestion1.DeepClone();

        // Act
        var result = dateQuestion1 == dateQuestion2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ComparingToADateQuestion_WithDifferentProperties_ReturnsFalse()
    {
        // Arrange

        var text1 = new TranslatedString
        {
            { _defaultLanguageCode, "some text" }
        };

        var text2 = new TranslatedString
        {
            { _defaultLanguageCode, "some text" }
        };

        var helptext1 = new TranslatedString
        {
            { _defaultLanguageCode, "other text" }
        };

        var helptext2 = new TranslatedString
        {
            { _defaultLanguageCode, "other different " }
        };

        var id = Guid.NewGuid();
        var dateQuestion1 = DateQuestion.Create(id, "C!", text1, helptext1);
        var dateQuestion2 = DateQuestion.Create(id, "C!", text2, helptext2);

        // Act
        var result = dateQuestion1 == dateQuestion2;

        // Assert
        result.Should().BeFalse();
    }
}
