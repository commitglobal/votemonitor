using Vote.Monitor.Core.Helpers;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormAggregate;

public partial class FormTests
{
    [Fact]
    public void WhenAddingTranslations_AndDuplicatedLanguageCodes_ThenOnlyNewLanguagesAreAdded()
    {
        // Arrange
        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.RO.Iso1, _languages, null, []);

        string[] newLanguages = [LanguagesList.RO.Iso1, LanguagesList.HU.Iso1];

        // Act
        form.AddTranslations(newLanguages);

        // Assert
        form.Languages.Should().BeEquivalentTo(_languages.Union(newLanguages));
    }

    [Fact]
    public void WhenAddingTranslations_AndNoNewLanguages_ThenFormStaysTheSame()
    {
        // Arrange
        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.RO.Iso1, _languages, null, []);

        var formBefore = form.DeepClone();

        // Act
        form.AddTranslations(_languages);

        // Assert
        form.Should().BeEquivalentTo(formBefore);
    }

    [Fact]
    public void WhenAddingTranslations_AndEmptyNewLanguages_ThenFormStaysTheSame()
    {
        // Arrange
        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.RO.Iso1, _languages, null, []);

        var formBefore = form.DeepClone();

        // Act
        form.AddTranslations([]);

        // Assert
        form.Should().BeEquivalentTo(formBefore);
    }

    [Fact]
    public void WhenAddingTranslations_ThenAddsTranslationsForFormTemplateDetails()
    {
        // Arrange
        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.RO.Iso1, _languages, null, []);

        string[] newLanguages = [LanguagesList.RO.Iso1, LanguagesList.HU.Iso1];

        // Act
        form.AddTranslations(newLanguages);

        // Assert
        form.Name.Should().Contain(LanguagesList.HU.Iso1, string.Empty);
        form.Description.Should().Contain(LanguagesList.HU.Iso1, string.Empty);
    }

    [Fact]
    public void WhenAddingTranslations_ThenAddsTranslationsForEachQuestion()
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
            LanguagesList.RO.Iso1, _languages, null, questions);

        string[] newLanguages = [LanguagesList.RO.Iso1, LanguagesList.HU.Iso1];

        // Act
        form.AddTranslations(newLanguages);

        // Assert
        form.Questions.Should().AllSatisfy(q => q.Text.Should().Contain(LanguagesList.HU.Iso1, string.Empty));
        form.Questions.Should().AllSatisfy(q => q.Helptext.Should().Contain(LanguagesList.HU.Iso1, string.Empty));

        form
            .Questions
            .OfType<TextQuestion>()
            .Should()
            .AllSatisfy(q => q.InputPlaceholder.Should().Contain(LanguagesList.HU.Iso1, string.Empty));

        form
            .Questions
            .OfType<NumberQuestion>()
            .Should()
            .AllSatisfy(q => q.InputPlaceholder.Should().Contain(LanguagesList.HU.Iso1, string.Empty));

        form
            .Questions
            .OfType<SingleSelectQuestion>()
            .Should()
            .AllSatisfy(q =>
                q.Options.Should().AllSatisfy(o => o.Text.Should().Contain(LanguagesList.HU.Iso1, string.Empty)));

        form
            .Questions
            .OfType<MultiSelectQuestion>()
            .Should()
            .AllSatisfy(q =>
                q.Options.Should().AllSatisfy(o => o.Text.Should().Contain(LanguagesList.HU.Iso1, string.Empty)));
    }

    [Fact]
    public void WhenAddingTranslations_ThenRecomputesLanguagesTranslationsStatus()
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
            LanguagesList.RO.Iso1, _languages, null, questions);

        string[] newLanguages = [LanguagesList.HU.Iso1];

        // Act
        form.AddTranslations(newLanguages);

        // Assert
        form.LanguagesTranslationStatus.Should().HaveCount(3);
        form.LanguagesTranslationStatus[LanguagesList.RO.Iso1].Should().Be(TranslationStatus.Translated);
        form.LanguagesTranslationStatus[LanguagesList.EN.Iso1].Should().Be(TranslationStatus.Translated);
        form.LanguagesTranslationStatus[LanguagesList.HU.Iso1].Should().Be(TranslationStatus.MissingTranslations);
    }
}
