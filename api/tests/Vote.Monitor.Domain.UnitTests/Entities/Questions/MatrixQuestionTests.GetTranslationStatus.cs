using Vote.Monitor.Core.Helpers;

namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class MatrixQuestionTests
{
    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void GetTranslationStatus_WithMissingTranslationForText_ReturnsMissingTranslation(string emptyString)
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString(emptyString);
        MatrixOption[] options = [.. new MatrixOptionFaker().Generate(3)];
        MatrixRow[] rows = [.. new MatrixRowFaker().Generate(3)];

        var matrixQuestion = MatrixQuestion.Create(id, "C!", text, null, null, options, rows);

        // Act
        var status = matrixQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

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
        MatrixOption[] options = [.. new MatrixOptionFaker().Generate(3)];
        MatrixRow[] rows = [.. new MatrixRowFaker().Generate(3)];

        var matrixQuestion = MatrixQuestion.Create(id, "C!", text, helptext, null, options, rows);

        // Act
        var status = matrixQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

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
        string[] languages = [_defaultLanguageCode, _languageCode];

        
        MatrixOption[] options = [.. new MatrixOptionFaker(languageList: languages).Generate(3)];
        MatrixRow[] rows = [.. new MatrixRowFaker(languageList: languages).Generate(3)];
        var matrixQuestion = MatrixQuestion.Create(id, "C!", text, helptext, null, options, rows);

        // Act
        var status = matrixQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

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
        string[] languages = [_defaultLanguageCode, _languageCode];
        
        MatrixOption[] options = [.. new MatrixOptionFaker(languageList: languages).Generate(3)];
        MatrixRow[] rows = [.. new MatrixRowFaker(languageList: languages).Generate(3)];

        var matrixQuestion = MatrixQuestion.Create(id, "C!", text, helptext, null, options, rows);

        // Act
        var status = matrixQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

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
        var nonTranslatedOption = MatrixOption.Create(Guid.NewGuid(),  CreateTranslatedString(emptyString), false);
        var translatedOption = MatrixOption.Create(Guid.NewGuid(), new TranslatedString
        {
            [_defaultLanguageCode] = "some option",
            [_languageCode] = "some option translated"
        }, false);
        
        string[] languages = [_defaultLanguageCode, _languageCode];
        
        MatrixRow[] rows = [.. new MatrixRowFaker(languageList: languages).Generate(3)];
        
        var matrixQuestion = MatrixQuestion.Create(id, "C!", text, helptext, null, [translatedOption, nonTranslatedOption], rows);

        // Act
        var status = matrixQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.MissingTranslations);
    }
    
    // to implement
    // GetTranslationStatus_WhenFullyTranslated_ReturnsTranslated()
    
    [Fact]
    public void GetTranslationStatus_WhenFullyTranslated_ReturnsTranslated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var helptext = CreateTranslatedString("some other text");
        
        string[] languages = [_defaultLanguageCode, _languageCode];
        MatrixOption[] options = [.. new MatrixOptionFaker(languageList: languages).Generate(3)];
        MatrixRow[] rows = [.. new MatrixRowFaker(languageList: languages).Generate(3)];
        
        var matrixQuestion = MatrixQuestion.Create(id, "C!", text, helptext, null, options, rows );

        // Act
        var status = matrixQuestion.GetTranslationStatus(_defaultLanguageCode, _languageCode);

        // Assert
        status.Should().Be(TranslationStatus.Translated);
    }
    
}
