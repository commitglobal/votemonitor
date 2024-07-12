using Vote.Monitor.Core.Constants;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;
using Vote.Monitor.TestUtils.Utils;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormAggregate;

public partial class FormTests
{
    [Fact]
    public void WhenAddingTranslations_AndDuplicatedLanguageCodes_ThenOnlyNewLanguagesAreAdded()
    {
        // Arrange
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", name, description,
            LanguagesList.RO.Iso1, languages, []);

        string[] newLanguages = [LanguagesList.RO.Iso1, LanguagesList.HU.Iso1];

        // Act
        form.AddTranslations(newLanguages);

        // Assert
        form.Languages.Should().BeEquivalentTo(languages.Union(newLanguages));
    }

    [Fact]
    public void WhenAddingTranslations_AndNoNewLanguages_ThenFormStaysTheSame()
    {
        // Arrange
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", name, description,
            LanguagesList.RO.Iso1, languages, []);
        
        var formBefore = form.DeepClone();

        // Act
        form.AddTranslations(languages);

        // Assert
        form.Should().BeEquivalentTo(formBefore);
    }

    [Fact]
    public void WhenAddingTranslations_AndEmptyNewLanguages_ThenFormStaysTheSame()
    {
        // Arrange
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", name, description,
            LanguagesList.RO.Iso1, languages, []);

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
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", name, description,
            LanguagesList.RO.Iso1, languages, []);

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
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", name, description,
            LanguagesList.RO.Iso1, languages, []);
        
        BaseQuestion[] questions =
        [
            new TextQuestionFaker(languages).Generate(),
            new NumberQuestionFaker(languages).Generate(),
            new DateQuestionFaker(languages).Generate(),
            new RatingQuestionFaker(languageList: languages).Generate(),
            new SingleSelectQuestionFaker(languageList: languages).Generate(),
            new MultiSelectQuestionFaker(languageList: languages).Generate(),
        ];

        form.UpdateDetails(form.Code, form.Name, form.Description, form.FormType, form.DefaultLanguage,
            form.Languages, questions);

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
}