namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class MatrixQuestionTests
{
    [Fact]
    public void TrimTranslations_RemovesUnusedTranslations()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var helptext = CreateTranslatedString("some other text");
        
        string[] languages = [_defaultLanguageCode, _languageCode];

        
        MatrixRow[] rows = [.. new MatrixRowFaker(languageList: languages).Generate(1)];
        
        var matrixQuestion = MatrixQuestion.Create(id, "C!", text, helptext, null, [_translatedOption],[_translatedRow]);
        
        
        // Act
        matrixQuestion.TrimTranslations([_defaultLanguageCode]);

        // Assert
        matrixQuestion.Text.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        matrixQuestion.Text.Should().HaveCount(1);
        matrixQuestion.Helptext.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        matrixQuestion.Options.First().Text.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some option"));
        matrixQuestion.Rows.First().Text.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some option"));

    }
    
    [Fact]
    public void TrimTranslations_RemovesUnusedTranslations_Ignores_Null()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");     
        string[] languages = [_defaultLanguageCode, _languageCode];
        
        var matrixQuestion = MatrixQuestion.Create(id, "C!", text, null, null, [_translatedOption],[_translatedRow]);
    
        // Act
        matrixQuestion.TrimTranslations([_defaultLanguageCode]);
    
        // Assert
        matrixQuestion.Text.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        
        matrixQuestion.Options.First().Text.Should().HaveCount(1);
        matrixQuestion.Options.First().Text.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some option"));

        matrixQuestion.Rows.First().Text.Should().HaveCount(1);
        matrixQuestion.Rows.First().Text.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some option"));
        matrixQuestion.Helptext.Should().BeNull();
    }
}
