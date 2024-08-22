namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class RatingQuestionTests
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

        var ratingQuestion = RatingQuestion.Create(id, "C!", text, RatingScale.OneTo3);

        // Act
        var status = ratingQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

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

        var ratingQuestion = RatingQuestion.Create(id, "C!", text, RatingScale.OneTo3, helptext);

        // Act
        var status = ratingQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

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
        var ratingQuestion = RatingQuestion.Create(id, "C!", text, RatingScale.OneTo3, helptext);

        // Act
        var status = ratingQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

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

        var ratingQuestion = RatingQuestion.Create(id, "C!", text, RatingScale.OneTo3, helptext);

        // Act
        var status = ratingQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.Translated);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void GetTranslationStatus_WithMissingTranslationForLowerLabel_ReturnsMissingTranslation(string emptyString)
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var lowerLabel = CreateTranslatedString(emptyString);

        var ratingQuestion = RatingQuestion.Create(id, "C!", text, RatingScale.OneTo3, lowerLabel: lowerLabel);

        // Act
        var status = ratingQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.MissingTranslations);
    }

    [Fact]
    public void GetTranslationStatus_NullLowerLabel_ReturnsTranslated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        TranslatedString? lowerLabel = null;
        var ratingQuestion = RatingQuestion.Create(id, "C!", text, RatingScale.OneTo3, lowerLabel: lowerLabel);

        // Act
        var status = ratingQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.Translated);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void GetTranslationStatus_WithLowerLabelEmptyForBaseLanguage_ReturnsTranslated(string emptyString)
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var lowerLabel = new TranslatedString
        {
            [_defaultLanguageCode] = emptyString,
            [_languageCode] = "some helptext"
        };

        var ratingQuestion = RatingQuestion.Create(id, "C!", text, RatingScale.OneTo3, lowerLabel: lowerLabel);

        // Act
        var status = ratingQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.Translated);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void GetTranslationStatus_WithMissingTranslationForUpperLabel_ReturnsMissingTranslation(string emptyString)
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var upperLabel = CreateTranslatedString(emptyString);

        var ratingQuestion = RatingQuestion.Create(id, "C!", text, RatingScale.OneTo3, upperLabel: upperLabel);

        // Act
        var status = ratingQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.MissingTranslations);
    }

    [Fact]
    public void GetTranslationStatus_NullUpperLabel_ReturnsTranslated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        TranslatedString? upperLabel = null;
        var ratingQuestion = RatingQuestion.Create(id, "C!", text, RatingScale.OneTo3, upperLabel: upperLabel);

        // Act
        var status = ratingQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.Translated);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void GetTranslationStatus_WithUpperLabelEmptyForBaseLanguage_ReturnsTranslated(string emptyString)
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var upperLabel = new TranslatedString
        {
            [_defaultLanguageCode] = emptyString,
            [_languageCode] = "some helptext"
        };

        var ratingQuestion = RatingQuestion.Create(id, "C!", text, RatingScale.OneTo3, upperLabel: upperLabel);

        // Act
        var status = ratingQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

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

        var ratingQuestion = RatingQuestion.Create(id, "C!", text, RatingScale.OneTo3, helptext);

        // Act
        var status = ratingQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.Translated);
    }
}