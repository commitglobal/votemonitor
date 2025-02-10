using Vote.Monitor.Core.Helpers;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormAggregate;

public partial class FormTests
{
    [Fact]
    public void WhenRemovingTranslation_AndFormTemplateDoesNotHaveIt_ThenFormTemplateStaysTheSame()
    {
        // Arrange
        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.RO.Iso1, _languages, null, displayOrder: 0, []);

        var formBefore = form.DeepClone();

        // Act
        form.RemoveTranslation("UNKNOWN");

        // Assert
        form.Should().BeEquivalentTo(formBefore);
    }

    [Fact]
    public void WhenRemovingTranslation_ThenRemovesTranslationForFormTemplateDetails()
    {
        // Arrange
        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.RO.Iso1, _languages, null, displayOrder: 0, []);

        // Act
        form.RemoveTranslation(LanguagesList.UK.Iso1);

        // Assert
        form.Languages.Should().BeEquivalentTo([LanguagesList.RO.Iso1, LanguagesList.EN.Iso1]);
        form.Name.Should().NotContainKey(LanguagesList.UK.Iso1);
        form.Description.Should().NotContainKey(LanguagesList.UK.Iso1);
    }

    [Fact]
    public void WhenRemovingTranslation_AndDefaultLanguageIsRemoved_ThenException()
    {
        // Arrange
        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.RO.Iso1, _languages, null, displayOrder: 0, []);

        // Act
        var act = () => form.RemoveTranslation(LanguagesList.RO.Iso1);

        // Assert
        act
            .Should().Throw<ArgumentException>()
            .WithMessage("Cannot remove default language");
    }

    [Fact]
    public void WhenRemovingTranslation_ThenRemovesTranslationForEachQuestion()
    {
        // Arrange
        BaseQuestion[] questions =
        [
            new TextQuestionFaker(_languages).Generate(),
            new NumberQuestionFaker(_languages).Generate(),
            new DateQuestionFaker(_languages).Generate(),
            new RatingQuestionFaker(languageList: _languages).Generate(),
            new SingleSelectQuestionFaker(languageList: _languages).Generate(),
            new MultiSelectQuestionFaker(languageList: _languages).Generate()
        ];

        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.RO.Iso1, _languages, null, displayOrder: 0, questions);

        // Act
        form.RemoveTranslation(LanguagesList.UK.Iso1);

        // Assert
        form.Questions.Should().AllSatisfy(q => q.Text.Should().NotContainKey(LanguagesList.UK.Iso1));
        form.Questions.Should().AllSatisfy(q => q.Helptext.Should().NotContainKey(LanguagesList.UK.Iso1));

        form
            .Questions
            .OfType<TextQuestion>()
            .Should()
            .AllSatisfy(q => q.InputPlaceholder.Should().NotContainKey(LanguagesList.UK.Iso1));

        form
            .Questions
            .OfType<NumberQuestion>()
            .Should()
            .AllSatisfy(q => q.InputPlaceholder.Should().NotContainKey(LanguagesList.UK.Iso1));

        form
            .Questions
            .OfType<SingleSelectQuestion>()
            .Should()
            .AllSatisfy(q => q.Options.Should().AllSatisfy(o => o.Text.Should().NotContainKey(LanguagesList.UK.Iso1)));

        form
            .Questions
            .OfType<MultiSelectQuestion>()
            .Should()
            .AllSatisfy(q => q.Options.Should().AllSatisfy(o => o.Text.Should().NotContainKey(LanguagesList.UK.Iso1)));
    }

    [Fact]
    public void WhenRemovingTranslation_ThenRecomputesLanguagesTranslationsStatus()
    {
        // Arrange
        BaseQuestion[] questions =
        [
            new TextQuestionFaker(_languages).Generate(),
            new NumberQuestionFaker(_languages).Generate(),
            new DateQuestionFaker(_languages).Generate(),
            new RatingQuestionFaker(languageList: _languages).Generate(),
            new SingleSelectQuestionFaker(languageList: _languages).Generate(),
            new MultiSelectQuestionFaker(languageList: _languages).Generate()
        ];

        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.RO.Iso1, _languages, null, displayOrder: 0, questions);

        // Act
        form.RemoveTranslation(LanguagesList.EN.Iso1);

        // Assert
        form.LanguagesTranslationStatus.Should().HaveCount(1);
        form.LanguagesTranslationStatus[LanguagesList.RO.Iso1].Should().Be(TranslationStatus.Translated);
    }
}
