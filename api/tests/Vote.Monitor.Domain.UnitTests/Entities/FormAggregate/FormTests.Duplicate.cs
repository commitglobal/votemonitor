namespace Vote.Monitor.Domain.UnitTests.Entities.FormAggregate;

public partial class FormTests
{
    [Fact]
    public void WhenDuplicate_ThenCreatesNewDraftedFormTemplate()
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
            LanguagesList.RO.Iso1, Languages, null, questions);

        form.Publish();

        // Act
        var newFormTemplate = form.Duplicate();

        // Assert
        newFormTemplate.Id.Should().NotBe(form.Id);
        newFormTemplate.Code.Should().Be(form.Code);
        newFormTemplate.Description.Should().BeEquivalentTo(form.Description);
        newFormTemplate.DefaultLanguage.Should().BeEquivalentTo(form.DefaultLanguage);
        newFormTemplate.Languages.Should().BeEquivalentTo(form.Languages);
        newFormTemplate.Questions.Should().BeEquivalentTo(form.Questions);
        newFormTemplate.Status.Should().Be(FormStatus.Drafted);
        newFormTemplate.FormType.Should().Be(form.FormType);
    }

    [Theory]
    [MemberData(nameof(FormTestsTestData.PartiallyTranslatedQuestionsTestData), MemberType = typeof(FormTestsTestData))]
    public void WhenDuplicate_ThenRecomputesLanguagesTranslationsStatus(BaseQuestion[] questions)
    {
        // Arrange
        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", _name, _description,
            LanguagesList.EN.Iso1, Languages, null, questions);

        // Act
        var newForm = form.Duplicate();

        // Assert
        newForm.LanguagesTranslationStatus.Should().HaveCount(2);
        newForm.LanguagesTranslationStatus[LanguagesList.EN.Iso1].Should().Be(TranslationStatus.Translated);
        newForm.LanguagesTranslationStatus[LanguagesList.RO.Iso1].Should().Be(TranslationStatus.MissingTranslations);
    }
}