using Vote.Monitor.Core.Helpers;

namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class RatingQuestionTests
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
    public void ComparingToARatingQuestion_WithSameProperties_ReturnsTrue()
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

        var id = Guid.NewGuid();

        var ratingQuestion1 = RatingQuestion.Create(id, "C!", text1, RatingScale.OneTo10, helptext1);
        var ratingQuestion2 = RatingQuestion.Create(id, "C!", text2, RatingScale.OneTo10, helptext2);

        // Act
        var result = ratingQuestion1 == ratingQuestion2;

        // Assert
        result.Should().BeFalse();
    }
}
