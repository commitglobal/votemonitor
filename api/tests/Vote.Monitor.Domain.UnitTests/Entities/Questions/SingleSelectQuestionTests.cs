using Vote.Monitor.Core.Helpers;

namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class SingleSelectQuestionTests
{
    private readonly string _defaultLanguageCode = LanguagesList.EN.Iso1;
    private readonly string _languageCode = LanguagesList.RO.Iso1;
    private readonly SelectOption _translatedOption;

    public SingleSelectQuestionTests()
    {
        _translatedOption = SelectOption.Create(Guid.NewGuid(), new TranslatedString
        {
            [_defaultLanguageCode] = "some option",
            [_languageCode] = "some option translated"
        });
    }

    private TranslatedString CreateTranslatedString(string value)
    {
        return new TranslatedString
        {
            [_defaultLanguageCode] = "some text for default language",
            [_languageCode] = value
        };
    }
    
    [Fact]
    public void ComparingToASingleSelectQuestion_WithSameProperties_ReturnsTrue()
    {
        // Arrange
        var text = new TranslatedString
        {
            {_defaultLanguageCode, "some text"}
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
