namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class DateQuestionTests
{
    [Fact]
    public void TrimTranslations_RemovesUnusedTranslations()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var helptext = CreateTranslatedString("some other text");
        var dateQuestion = DateQuestion.Create(id, "C!", text, helptext);

        // Act
        dateQuestion.TrimTranslations([_defaultLanguageCode]);

        // Assert
        dateQuestion.Text.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        dateQuestion.Helptext.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
    }
    
    [Fact]
    public void TrimTranslations_RemovesUnusedTranslations_Ignores_Null()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        TranslatedString? helptext = null;
        var dateQuestion = DateQuestion.Create(id, "C!", text, helptext);

        // Act
        dateQuestion.TrimTranslations([_defaultLanguageCode]);

        // Assert
        dateQuestion.Text.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        dateQuestion.Helptext.Should().BeNull();
    }
}
