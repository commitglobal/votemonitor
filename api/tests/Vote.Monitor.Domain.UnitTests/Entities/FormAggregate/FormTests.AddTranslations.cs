﻿using Vote.Monitor.TestUtils.Utils;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormAggregate;

public partial class FormTests
{
    [Fact]
    public void WhenAddingTranslations_AndDuplicatedLanguageCodes_ThenOnlyNewLanguagesAreAdded()
    {
        // Arrange
        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.RO.Iso1, Languages, []);

        string[] newLanguages = [LanguagesList.RO.Iso1, LanguagesList.HU.Iso1];

        // Act
        form.AddTranslations(newLanguages);

        // Assert
        form.Languages.Should().BeEquivalentTo(Languages.Union(newLanguages));
    }

    [Fact]
    public void WhenAddingTranslations_AndNoNewLanguages_ThenFormStaysTheSame()
    {
        // Arrange
        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.RO.Iso1, Languages, []);

        var formBefore = form.DeepClone();

        // Act
        form.AddTranslations(Languages);

        // Assert
        form.Should().BeEquivalentTo(formBefore);
    }

    [Fact]
    public void WhenAddingTranslations_AndEmptyNewLanguages_ThenFormStaysTheSame()
    {
        // Arrange
        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.RO.Iso1, Languages, []);

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
            LanguagesList.RO.Iso1, Languages, []);

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
        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.RO.Iso1, Languages, []);

        BaseQuestion[] questions =
        [
            new TextQuestionFaker(Languages).Generate(),
            new NumberQuestionFaker(Languages).Generate(),
            new DateQuestionFaker(Languages).Generate(),
            new RatingQuestionFaker(languageList: Languages).Generate(),
            new SingleSelectQuestionFaker(languageList: Languages).Generate(),
            new MultiSelectQuestionFaker(languageList: Languages).Generate(),
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

    [Fact]
    public void WhenAddingTranslations_ThenRecomputesLanguagesTranslationsStatus()
    {
        // Arrange
        BaseQuestion[] questions =
        [
            new TextQuestionFaker(Languages).Generate(),
            new NumberQuestionFaker(Languages).Generate(),
            new DateQuestionFaker(Languages).Generate(),
            new RatingQuestionFaker(languageList: Languages).Generate(),
            new SingleSelectQuestionFaker(languageList: Languages).Generate(),
            new MultiSelectQuestionFaker(languageList: Languages).Generate(),
        ];

        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.RO.Iso1, Languages, questions);

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