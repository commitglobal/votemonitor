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
    public void WhenRemovingTranslation_AndFormTemplateDoesNotHaveIt_ThenFormTemplateStaysTheSame()
    {
        // Arrange
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", name, description,
            LanguagesList.RO.Iso1, languages, []);

        var formBefore = form.DeepClone();

        // Act
        form.RemoveTranslation("UNKNOWN");

        // Assert
        form.Should().BeEquivalentTo(formBefore);
    }

    [Fact]
    public void WhenRemovingTranslation_ThenRemovesTranslationForFormTemplateDetails()
    {
        // Arrange
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", name, description,
            LanguagesList.RO.Iso1, languages, []);

        // Act
        form.RemoveTranslation(LanguagesList.UK.Iso1);

        // Assert
        form.Languages.Should().BeEquivalentTo([LanguagesList.RO.Iso1, LanguagesList.EN.Iso1]);
        form.Name.Should().NotContainKey(LanguagesList.UK.Iso1);
        form.Description.Should().NotContainKey(LanguagesList.UK.Iso1);
    }

    [Fact]
    public void WhenRemovingTranslation_AndDefaultLanguageIsRemoved_ThenException()
    {
        // Arrange
        string[] languages = [LanguagesList.RO.Iso1, LanguagesList.EN.Iso1, LanguagesList.UK.Iso1];
        var name = new TranslatedStringFaker(languages).Generate();
        var description = new TranslatedStringFaker(languages).Generate();

        var form = Form.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Voting, "code", name, description,
            LanguagesList.RO.Iso1, languages, []);

        // Act
        Action act = () => form.RemoveTranslation(LanguagesList.RO.Iso1);

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

        form.UpdateDetails(form.Code, form.Name, form.Description, form.FormType, form.DefaultLanguage, form.Languages,
            questions);

        // Act
        form.RemoveTranslation(LanguagesList.UK.Iso1);

        // Assert
        form.Questions.Should().AllSatisfy(q => q.Text.Should().NotContainKey(LanguagesList.UK.Iso1));
        form.Questions.Should().AllSatisfy(q => q.Helptext.Should().NotContainKey(LanguagesList.UK.Iso1));

        form
            .Questions
            .OfType<TextQuestion>()
            .Should()
            .AllSatisfy(q => q.InputPlaceholder.Should().NotContainKey(LanguagesList.UK.Iso1));

        form
            .Questions
            .OfType<NumberQuestion>()
            .Should()
            .AllSatisfy(q => q.InputPlaceholder.Should().NotContainKey(LanguagesList.UK.Iso1));

        form
            .Questions
            .OfType<SingleSelectQuestion>()
            .Should()
            .AllSatisfy(q => q.Options.Should().AllSatisfy(o => o.Text.Should().NotContainKey(LanguagesList.UK.Iso1)));

        form
            .Questions
            .OfType<MultiSelectQuestion>()
            .Should()
            .AllSatisfy(q => q.Options.Should().AllSatisfy(o => o.Text.Should().NotContainKey(LanguagesList.UK.Iso1)));
    }
}