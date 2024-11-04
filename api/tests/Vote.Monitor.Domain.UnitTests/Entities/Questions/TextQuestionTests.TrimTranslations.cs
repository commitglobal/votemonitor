namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class TextQuestionTests
{
    
    [Fact]
    public void TrimTranslations_RemovesUnusedTranslations()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var helptext = CreateTranslatedString("some other text");
        var inputPlaceholder = CreateTranslatedString("some other other text");
        var textQuestion = TextQuestion.Create(id, "C!", text, helptext, inputPlaceholder);

        // Act
        textQuestion.TrimTranslations([_defaultLanguageCode]);

        // Assert
        textQuestion.Text.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        textQuestion.Helptext.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        textQuestion.InputPlaceholder.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
    }
    
    [Fact]
    public void TrimTranslations_RemovesUnusedTranslations_Ignores_Null()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        
        var textQuestion = TextQuestion.Create(id, "C!", text, null, null);

        // Act
        textQuestion.TrimTranslations([_defaultLanguageCode]);

        // Assert
        textQuestion.Text.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        textQuestion.Helptext.Should().BeNull();
        textQuestion.InputPlaceholder.Should().BeNull();
    }
}
