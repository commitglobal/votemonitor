namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class MultiSelectQuestionTests
{
    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void GetTranslationStatus_WithMissingTranslationForText_ReturnsMissingTranslation(string emptyString)
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString(emptyString);
        
        var multiSelectQuestion = MultiSelectQuestion.Create(id, "C!", text, [_translatedOption]);

        // Act
        var status = multiSelectQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

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
        
        var multiSelectQuestion = MultiSelectQuestion.Create(id, "C!", text, [_translatedOption], helptext);

        // Act
        var status = multiSelectQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

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
        
        var multiSelectQuestion = MultiSelectQuestion.Create(id, "C!", text, [_translatedOption], helptext);

        // Act
        var status = multiSelectQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

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

        var multiSelectQuestion = MultiSelectQuestion.Create(id, "C!", text, [_translatedOption], helptext);

        // Act
        var status = multiSelectQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.Translated);
    }
    
    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void GetTranslationStatus_WithMissingTranslationForOption_ReturnsMissingTranslation(string emptyString)
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var helptext = CreateTranslatedString("some helptext");
        var nonTranslatedOption = SelectOption.Create(Guid.NewGuid(),  CreateTranslatedString(emptyString));
        
        var multiSelectQuestion = MultiSelectQuestion.Create(id, "C!", text, [_translatedOption, nonTranslatedOption], helptext);

        // Act
        var status = multiSelectQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.MissingTranslations);
    }

    [Fact]
    public void GetTranslationStatus_WhenFullyTranslated_ReturnsTranslated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var helptext = CreateTranslatedString("some other text");
        
        var multiSelectQuestion = MultiSelectQuestion.Create(id, "C!", text, [_translatedOption], helptext);

        // Act
        var status = multiSelectQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.Translated);
    }
}
