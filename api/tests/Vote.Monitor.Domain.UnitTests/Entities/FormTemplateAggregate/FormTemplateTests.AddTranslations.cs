﻿using Vote.Monitor.Core.Constants;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;
using Vote.Monitor.TestUtils.Utils;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormTemplateAggregate;
public partial class FormTemplateTests
{
    [Fact]
    public void WhenAddingTranslations_AndDuplicatedLanguageCodes_ThenOnlyNewLanguagesAreAdded()
    {
        // Arrange
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var formTemplate = FormTemplate.Create(FormTemplateType.Voting, "code", LanguagesList.RO.Iso1, name,
            description, languages);

        string[] newLanguages = [LanguagesList.RO.Iso1, LanguagesList.HU.Iso1];

        // Act
        formTemplate.AddTranslations(newLanguages);

        // Assert
        formTemplate.Languages.Should().BeEquivalentTo(languages.Union(newLanguages));
    }

    [Fact]
    public void WhenAddingTranslations_AndNoNewLanguages_ThenFormStaysTheSame()
    {
        // Arrange
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var formTemplate = FormTemplate.Create(FormTemplateType.Voting, "code", LanguagesList.RO.Iso1, name,
            description, languages);
        var formBefore = formTemplate.DeepClone();

        // Act
        formTemplate.AddTranslations(languages);

        // Assert
        formTemplate.Should().BeEquivalentTo(formBefore);
    }

    [Fact]
    public void WhenAddingTranslations_AndEmptyNewLanguages_ThenFormStaysTheSame()
    {
        // Arrange
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var formTemplate = FormTemplate.Create(FormTemplateType.Voting, "code", LanguagesList.RO.Iso1, name,
            description, languages);

        var formBefore = formTemplate.DeepClone();

        // Act
        formTemplate.AddTranslations([]);

        // Assert
        formTemplate.Should().BeEquivalentTo(formBefore);
    }

    [Fact]
    public void WhenAddingTranslations_ThenAddsTranslationsForFormTemplateDetails()
    {
        // Arrange
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var formTemplate = FormTemplate.Create(FormTemplateType.Voting, "code", LanguagesList.RO.Iso1, name,
            description, languages);

        string[] newLanguages = [LanguagesList.RO.Iso1, LanguagesList.HU.Iso1];

        // Act
        formTemplate.AddTranslations(newLanguages);

        // Assert
        formTemplate.Name.Should().Contain(LanguagesList.HU.Iso1, string.Empty);
        formTemplate.Description.Should().Contain(LanguagesList.HU.Iso1, string.Empty);
    }

    [Fact]
    public void WhenAddingTranslations_ThenAddsTranslationsForEachQuestion()
    {
        // Arrange
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var formTemplate = FormTemplate.Create(FormTemplateType.Voting, "code", LanguagesList.RO.Iso1, name,
            description, languages);
        BaseQuestion[] questions =
        [
            new TextQuestionFaker(languages).Generate(),
            new NumberQuestionFaker(languages).Generate(),
            new DateQuestionFaker(languages).Generate(),
            new RatingQuestionFaker(languageList: languages).Generate(),
            new SingleSelectQuestionFaker(languageList: languages).Generate(),
            new MultiSelectQuestionFaker(languageList: languages).Generate(),
        ];

        formTemplate.UpdateDetails(formTemplate.Code, formTemplate.DefaultLanguage, formTemplate.Name, formTemplate.Description, formTemplate.FormTemplateType, formTemplate.Languages, questions);

        string[] newLanguages = [LanguagesList.RO.Iso1, LanguagesList.HU.Iso1];

        // Act
        formTemplate.AddTranslations(newLanguages);

        // Assert
        formTemplate.Questions.Should().AllSatisfy(q => q.Text.Should().Contain(LanguagesList.HU.Iso1, string.Empty));
        formTemplate.Questions.Should().AllSatisfy(q => q.Helptext.Should().Contain(LanguagesList.HU.Iso1, string.Empty));

        formTemplate
            .Questions
            .OfType<TextQuestion>()
            .Should()
            .AllSatisfy(q => q.InputPlaceholder.Should().Contain(LanguagesList.HU.Iso1, string.Empty));

        formTemplate
            .Questions
            .OfType<NumberQuestion>()
            .Should()
            .AllSatisfy(q => q.InputPlaceholder.Should().Contain(LanguagesList.HU.Iso1, string.Empty));

        formTemplate
            .Questions
            .OfType<SingleSelectQuestion>()
            .Should()
            .AllSatisfy(q => q.Options.Should().AllSatisfy(o => o.Text.Should().Contain(LanguagesList.HU.Iso1, string.Empty)));

        formTemplate
            .Questions
            .OfType<MultiSelectQuestion>()
            .Should()
            .AllSatisfy(q => q.Options.Should().AllSatisfy(o => o.Text.Should().Contain(LanguagesList.HU.Iso1, string.Empty)));
    }
}
