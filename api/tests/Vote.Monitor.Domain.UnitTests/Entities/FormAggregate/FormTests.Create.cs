namespace Vote.Monitor.Domain.UnitTests.Entities.FormAggregate;

public partial class FormTests
{
    [Theory]
    [MemberData(nameof(FormTestsTestData.PartiallyTranslatedQuestionsTestData), MemberType = typeof(FormTestsTestData))]
    public void WhenCreateForm_ThenRecomputesLanguagesTranslationsStatus(BaseQuestion[] questions)
    {
        // Arrange & Act
        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.EN.Iso1, _languages, null, displayOrder: 0, questions);

        // Assert
        form.LanguagesTranslationStatus.Should().HaveCount(2);
        form.LanguagesTranslationStatus[LanguagesList.EN.Iso1].Should().Be(TranslationStatus.Translated);
        form.LanguagesTranslationStatus[LanguagesList.RO.Iso1].Should().Be(TranslationStatus.MissingTranslations);
    }
}
