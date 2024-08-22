namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class DateQuestionTests
{
    private readonly string _defaultLanguageCode = LanguagesList.EN.Iso1;
    private readonly string _languageCode = LanguagesList.RO.Iso1;

    private TranslatedString CreateTranslatedString(string value)
    {
        return new TranslatedString
        {
            [_defaultLanguageCode] = "some text",
            [_languageCode] = value
        };
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void GetTranslationStatus_WithMissingTranslationForText_ReturnsMissingTranslation(string emptyString)
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString(emptyString);
        
        var dateQuestion = DateQuestion.Create(id, "C!", text);

        // Act
        var status = dateQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.MissingTranslations);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void GetTranslationStatus_WithMissingTranslationForHelptext_ReturnsMissingTranslation(string emptyString)
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var helptext = CreateTranslatedString(emptyString);
        
        var dateQuestion = DateQuestion.Create(id, "C!", text, helptext);

        // Act
        var status = dateQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.MissingTranslations);
    }

    [Fact]
    public void GetTranslationStatus_NullHelptext_ReturnsTranslated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        TranslatedString? helptext = null;
        var dateQuestion = DateQuestion.Create(id, "C!", text, helptext);

        // Act
        var status = dateQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.Translated);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void GetTranslationStatus_WithHelptextEmptyForBaseLanguage_ReturnsTranslated(string emptyString)
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var helptext = new TranslatedString
        {
            [_defaultLanguageCode] = emptyString,
            [_languageCode] = "some helptext"
        };

        var dateQuestion = DateQuestion.Create(id, "C!", text, helptext);

        // Act
        var status = dateQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.Translated);
    }

    [Fact]
    public void GetTranslationStatus_WhenFullyTranslated_ReturnsTranslated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var helptext = CreateTranslatedString("some other text");
        var dateQuestion = DateQuestion.Create(id, "C!", text, helptext);

        // Act
        var status = dateQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.Translated);
    }
}