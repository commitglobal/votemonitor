namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class NumberQuestionTests
{
    [Fact]
    public void TrimTranslations_RemovesUnusedTranslations()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var helptext = CreateTranslatedString("some other text");
        var inputPlaceholder = CreateTranslatedString("some other other text");
        var numberQuestion = NumberQuestion.Create(id, "C!", text, helptext, inputPlaceholder);

        // Act
        numberQuestion.TrimTranslations([_defaultLanguageCode]);

        // Assert
        numberQuestion.Text.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        numberQuestion.Helptext.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        numberQuestion.InputPlaceholder.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
    }
    
    [Fact]
    public void TrimTranslations_RemovesUnusedTranslations_Ignores_Null()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        
        var numberQuestion = NumberQuestion.Create(id, "C!", text, null, null);

        // Act
        numberQuestion.TrimTranslations([_defaultLanguageCode]);

        // Assert
        numberQuestion.Text.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        numberQuestion.Helptext.Should().BeNull();
        numberQuestion.InputPlaceholder.Should().BeNull();
    }
}
