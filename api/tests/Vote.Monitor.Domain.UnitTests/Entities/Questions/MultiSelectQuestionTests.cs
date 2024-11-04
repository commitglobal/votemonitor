using Vote.Monitor.Core.Helpers;

namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class MultiSelectQuestionTests
{
    private readonly string _defaultLanguageCode = LanguagesList.EN.Iso1;
    private readonly string _languageCode = LanguagesList.RO.Iso1;
    private readonly SelectOption _translatedOption;

    private TranslatedString CreateTranslatedString(string value)
    {
        return new TranslatedString { [_defaultLanguageCode] = "some text for default language", [_languageCode] = value };
    }

    public MultiSelectQuestionTests()
    {
        _translatedOption = SelectOption.Create(Guid.NewGuid(),
            new TranslatedString
            {
                [_defaultLanguageCode] = "some option", [_languageCode] = "some option translated"
            });
    }

    [Fact]
    public void ComparingToAMultiSelectQuestion_WithSameProperties_ReturnsTrue()
    {
        // Arrange
        var text = new TranslatedString { { _defaultLanguageCode, "some text" } };

        var helptext = new TranslatedString { { _defaultLanguageCode, "other text" } };

        SelectOption[] options = [.. new SelectOptionFaker().Generate(3)];

        var id = Guid.NewGuid();
        var multiSelectQuestion1 = MultiSelectQuestion.Create(id, "C!", text, options, helptext);
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
        var text1 = new TranslatedString { { _defaultLanguageCode, "some text" } };

        var text2 = new TranslatedString { { _defaultLanguageCode, "some text" } };

        var helptext1 = new TranslatedString { { _defaultLanguageCode, "other text" } };

        var helptext2 = new TranslatedString { { _defaultLanguageCode, "other different" } };

        SelectOption[] options1 = [.. new SelectOptionFaker().Generate(3)];
        SelectOption[] options2 = [.. new SelectOptionFaker().Generate(3)];

        var id = Guid.NewGuid();

        var textQuestion1 = MultiSelectQuestion.Create(id, "C!", text1, options1, helptext1);
        var textQuestion2 = MultiSelectQuestion.Create(id, "C!", text2, options2, helptext2);

        // Act
        var result = textQuestion1 == textQuestion2;

        // Assert
        result.Should().BeFalse();
    }
}
