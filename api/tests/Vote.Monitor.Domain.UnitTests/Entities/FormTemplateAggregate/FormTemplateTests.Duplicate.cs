using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormTemplateAggregate;

public partial class FormTests
{
    [Fact]
    public void WhenDuplicate_ThenCreatesNewDraftedFormTemplate()
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

        var formTemplate = FormTemplate.Create(FormType.Voting, "code", LanguagesList.RO.Iso1, name, description,
            languages, null, questions);
        formTemplate.Publish();

        // Act
        var newFormTemplate = formTemplate.Duplicate();

        // Assert
        newFormTemplate.Id.Should().NotBe(formTemplate.Id);
        newFormTemplate.Code.Should().Be(formTemplate.Code);
        newFormTemplate.Description.Should().BeEquivalentTo(formTemplate.Description);
        newFormTemplate.DefaultLanguage.Should().BeEquivalentTo(formTemplate.DefaultLanguage);
        newFormTemplate.Languages.Should().BeEquivalentTo(formTemplate.Languages);
        newFormTemplate.Questions.Should().BeEquivalentTo(formTemplate.Questions);
        newFormTemplate.Status.Should().Be(FormStatus.Drafted);
        newFormTemplate.FormType.Should().Be(formTemplate.FormType);
    }
}
