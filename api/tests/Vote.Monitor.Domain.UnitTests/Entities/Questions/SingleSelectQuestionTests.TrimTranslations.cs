namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class SingleSelectQuestionTests
{
    [Fact]
    public void TrimTranslations_RemovesUnusedTranslations()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var helptext = CreateTranslatedString("some other text");
        var singleSelectQuestion = SingleSelectQuestion.Create(id, "C!", text, [_translatedOption], helptext);
        
        // Act
        singleSelectQuestion.TrimTranslations([_defaultLanguageCode]);

        // Assert
        singleSelectQuestion.Text.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        singleSelectQuestion.Helptext.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        singleSelectQuestion.Options.First().Text.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some option"));
    }
    
    [Fact]
    public void TrimTranslations_RemovesUnusedTranslations_Ignores_Null()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");        
        var singleSelectQuestion = SingleSelectQuestion.Create(id, "C!", text, [_translatedOption]);

        // Act
        singleSelectQuestion.TrimTranslations([_defaultLanguageCode]);

        // Assert
        singleSelectQuestion.Text.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        singleSelectQuestion.Options.First().Text.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some option"));
        singleSelectQuestion.Helptext.Should().BeNull();
    }
}
