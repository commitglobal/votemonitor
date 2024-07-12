using Vote.Monitor.Core.Constants;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormAggregate;

public partial class FormTests
{
    [Fact]
    public void WhenDuplicate_ThenCreatesNewDraftedFormTemplate()
    {
        // Arrange
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", name, description,
            LanguagesList.RO.Iso1, languages, []);

        form.Publish();

        BaseQuestion[] questions =
        [
            new TextQuestionFaker(languages).Generate(),
            new NumberQuestionFaker(languages).Generate(),
            new DateQuestionFaker(languages).Generate(),
            new RatingQuestionFaker(languageList: languages).Generate(),
            new SingleSelectQuestionFaker(languageList: languages).Generate(),
            new MultiSelectQuestionFaker(languageList: languages).Generate(),
        ];

        form.UpdateDetails(form.Code, form.Name, form.Description, form.FormType, form.DefaultLanguage, form.Languages,
            questions);
        
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
}