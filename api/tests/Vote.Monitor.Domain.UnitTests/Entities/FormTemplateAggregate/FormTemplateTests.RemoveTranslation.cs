using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormTemplateAggregate;

public partial class FormTests
{
    [Fact]
    public void WhenRemovingTranslation_AndFormTemplateDoesNotHaveIt_ThenFormTemplateStaysTheSame()
    {
        // Arrange
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var formTemplate = FormTemplate.Create(FormType.Voting, "code", LanguagesList.RO.Iso1, name,
            description, languages, icon: null,
            []);

        var formBefore = formTemplate.DeepClone();

        // Act
        formTemplate.RemoveTranslation("UNKNOWN");

        // Assert
        formTemplate.Should().BeEquivalentTo(formBefore);
    }

    [Fact]
    public void WhenRemovingTranslation_ThenRemovesTranslationForFormTemplateDetails()
    {
        // Arrange
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var formTemplate = FormTemplate.Create(FormType.Voting, "code", LanguagesList.RO.Iso1, name,
            description, languages, icon: null,
            []);

        // Act
        formTemplate.RemoveTranslation(LanguagesList.UK.Iso1);

        // Assert
        formTemplate.Languages.Should().BeEquivalentTo([LanguagesList.RO.Iso1, LanguagesList.EN.Iso1]);
        formTemplate.Name.Should().NotContainKey(LanguagesList.UK.Iso1);
        formTemplate.Description.Should().NotContainKey(LanguagesList.UK.Iso1);
    }

    [Fact]
    public void WhenRemovingTranslation_AndDefaultLanguageIsRemoved_ThenException()
    {
        // Arrange
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var formTemplate = FormTemplate.Create(FormType.Voting, "code", LanguagesList.RO.Iso1, name,
            description, languages, icon: null,
            []);

        // Act
        var act = () => formTemplate.RemoveTranslation(LanguagesList.RO.Iso1);

        // Assert
        act
            .Should().Throw<ArgumentException>()
            .WithMessage("Cannot remove default language");
    }

    [Fact]
    public void WhenRemovingTranslation_ThenRemovesTranslationForEachQuestion()
    {
        // Arrange
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();
        BaseQuestion[] questions =
        [
            new TextQuestionFaker(languages).Generate(),
            new NumberQuestionFaker(languages).Generate(),
            new DateQuestionFaker(languages).Generate(),
            new RatingQuestionFaker(languageList: languages).Generate(),
            new SingleSelectQuestionFaker(languageList: languages).Generate(),
            new MultiSelectQuestionFaker(languageList: languages).Generate()
        ];

        var formTemplate = FormTemplate.Create(FormType.Voting, "code", LanguagesList.RO.Iso1, name,
            description, languages, icon: null,
            questions);

        // Act
        formTemplate.RemoveTranslation(LanguagesList.UK.Iso1);

        // Assert
        formTemplate.Questions.Should().AllSatisfy(q => q.Text.Should().NotContainKey(LanguagesList.UK.Iso1));
        formTemplate.Questions.Should().AllSatisfy(q => q.Helptext.Should().NotContainKey(LanguagesList.UK.Iso1));

        formTemplate
            .Questions
            .OfType<TextQuestion>()
            .Should()
            .AllSatisfy(q => q.InputPlaceholder.Should().NotContainKey(LanguagesList.UK.Iso1));

        formTemplate
            .Questions
            .OfType<NumberQuestion>()
            .Should()
            .AllSatisfy(q => q.InputPlaceholder.Should().NotContainKey(LanguagesList.UK.Iso1));

        formTemplate
            .Questions
            .OfType<RatingQuestion>()
            .Should()
            .AllSatisfy(q => q.LowerLabel.Should().NotContainKey(LanguagesList.UK.Iso1))
            .And
            .AllSatisfy(q => q.UpperLabel.Should().NotContainKey(LanguagesList.UK.Iso1));

        formTemplate
            .Questions
            .OfType<SingleSelectQuestion>()
            .Should()
            .AllSatisfy(q => q.Options.Should().AllSatisfy(o => o.Text.Should().NotContainKey(LanguagesList.UK.Iso1)));

        formTemplate
            .Questions
            .OfType<MultiSelectQuestion>()
            .Should()
            .AllSatisfy(q => q.Options.Should().AllSatisfy(o => o.Text.Should().NotContainKey(LanguagesList.UK.Iso1)));
    }
}
