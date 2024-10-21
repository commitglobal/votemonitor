namespace Vote.Monitor.Domain.UnitTests.Entities.FormAggregate;

public partial class FormTests
{
    [Theory]
    [MemberData(nameof(FormTestsTestData.PartiallyTranslatedQuestionsTestData), MemberType = typeof(FormTestsTestData))]
    public void WhenUpdating_ThenRecomputesLanguagesTranslationsStatus(BaseQuestion[] questions)
    {
        // Arrange
        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.RO.Iso1, Languages, null, []);
        // Act
        form.UpdateDetails(form.Code, form.Name, form.Description, form.FormType, form.DefaultLanguage, form.Languages,
            null, questions);

        // Assert
        form.LanguagesTranslationStatus.Should().HaveCount(2);
        form.LanguagesTranslationStatus[LanguagesList.EN.Iso1].Should().Be(TranslationStatus.Translated);
        form.LanguagesTranslationStatus[LanguagesList.RO.Iso1].Should().Be(TranslationStatus.MissingTranslations);
    }
}