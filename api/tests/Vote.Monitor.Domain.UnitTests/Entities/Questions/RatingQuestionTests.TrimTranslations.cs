namespace Vote.Monitor.Domain.UnitTests.Entities.Questions;

public partial class RatingQuestionTests
{
    [Fact]
    public void TrimTranslations_RemovesUnusedTranslations()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        var helptext = CreateTranslatedString("some other text");
        var lowerLabel = CreateTranslatedString("some other other text");
        var upperLabel = CreateTranslatedString("some other other text");
        var ratingQuestion = RatingQuestion.Create(id, "C!", text, RatingScale.OneTo3, helptext, lowerLabel, upperLabel);

        // Act
        ratingQuestion.TrimTranslations([_defaultLanguageCode]);

        // Assert
        ratingQuestion.Text.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        ratingQuestion.Helptext.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        ratingQuestion.LowerLabel.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        ratingQuestion.UpperLabel.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
    }
    
    [Fact]
    public void TrimTranslations_RemovesUnusedTranslations_Ignores_Null()
    {
        // Arrange
        var id = Guid.NewGuid();
        var text = CreateTranslatedString("some text");
        
        var ratingQuestion = RatingQuestion.Create(id, "C!", text, RatingScale.OneTo3);

        // Act
        ratingQuestion.TrimTranslations([_defaultLanguageCode]);

        // Assert
        ratingQuestion.Text.Should().HaveCount(1).And.Subject.Should().Contain(new KeyValuePair<string, string>(_defaultLanguageCode, "some text for default language"));
        ratingQuestion.Helptext.Should().BeNull();
        ratingQuestion.LowerLabel.Should().BeNull();
        ratingQuestion.UpperLabel.Should().BeNull();
    }
}
