﻿namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class TextQuestionTests
{
    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void GetTranslationStatus_WithMissingTranslationForName_ReturnsMissingTranslation(string emptyString)
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString(emptyString);
        var helptext = CreateTranslatedString("some text");
        var inputPlaceholder = CreateTranslatedString("some placeholder");

        var textQuestion = TextQuestion.Create(id, "C!", text, helptext, inputPlaceholder);

        // Act
        var status = textQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

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
        var inputPlaceholder = CreateTranslatedString("some placeholder");

        var textQuestion = TextQuestion.Create(id, "C!", text, helptext, inputPlaceholder);

        // Act
        var status = textQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

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
        var inputPlaceholder = CreateTranslatedString("some placeholder");

        var textQuestion = TextQuestion.Create(id, "C!", text, helptext, inputPlaceholder);

        // Act
        var status = textQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

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
        var inputPlaceholder = CreateTranslatedString("some placeholder");

        var textQuestion = TextQuestion.Create(id, "C!", text, helptext, inputPlaceholder);

        // Act
        var status = textQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.Translated);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void GetTranslationStatus_WithMissingTranslationForInputPlaceholder_ReturnsMissingTranslation(
        string emptyString)
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var helptext = CreateTranslatedString("some helptext");
        var inputPlaceholder = CreateTranslatedString(emptyString);

        var textQuestion = TextQuestion.Create(id, "C!", text, helptext, inputPlaceholder);

        // Act
        var status = textQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.MissingTranslations);
    }

    [Fact]
    public void GetTranslationStatus_NullInputPlaceholder_ReturnsTranslated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var helptext = CreateTranslatedString("some helptext");
        TranslatedString? inputPlaceholder = null;

        var textQuestion = TextQuestion.Create(id, "C!", text, helptext, inputPlaceholder);

        // Act
        var status = textQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.Translated);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void GetTranslationStatus_WithInputPlaceholderEmptyForBaseLanguage_ReturnsTranslated(string emptyString)
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var helptext = CreateTranslatedString("some helptext");
        var inputPlaceholder = new TranslatedString
        {
            [_defaultLanguageCode] = emptyString,
            [_languageCode] = "input placeholder"
        };

        var textQuestion = TextQuestion.Create(id, "C!", text, helptext, inputPlaceholder);

        // Act
        var status = textQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

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
        var inputPlaceholder = CreateTranslatedString("some placeholder");

        var textQuestion = TextQuestion.Create(id, "C!", text, helptext, inputPlaceholder);

        // Act
        var status = textQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.Translated);
    }
}
