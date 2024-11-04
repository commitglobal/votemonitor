namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class MultiSelectQuestionTests
{
    [Fact]
    public void TrimTranslations_RemovesUnusedTranslations()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var helptext = CreateTranslatedString("some other text");
        var multiSelectQuestion = MultiSelectQuestion.Create(id, "C!", text, [_translatedOption], helptext);

        // Act
        multiSelectQuestion.TrimTranslations([_defaultLanguageCode]);

        // Assert
        multiSelectQuestion.Text.Should().HaveCount(1).And.Subject.Should()
            .Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        multiSelectQuestion.Helptext.Should().HaveCount(1).And.Subject.Should()
            .Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        multiSelectQuestion.Options.First().Text.Should().HaveCount(1).And.Subject.Should()
            .Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some option"));
    }

    [Fact]
    public void TrimTranslations_RemovesUnusedTranslations_Ignores_Null()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var multiSelectQuestion = MultiSelectQuestion.Create(id, "C!", text, [_translatedOption]);

        // Act
        multiSelectQuestion.TrimTranslations([_defaultLanguageCode]);

        // Assert
        multiSelectQuestion.Text.Should().HaveCount(1).And.Subject.Should()
            .Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        multiSelectQuestion.Options.First().Text.Should().HaveCount(1).And.Subject.Should()
            .Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some option"));
        multiSelectQuestion.Helptext.Should().BeNull();
    }
}
