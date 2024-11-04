using FluentValidation;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Form = Vote.Monitor.Domain.Entities.FormTemplateAggregate.Form;

namespace Vote.Monitor.Domain.UnitTests.Entities.FormAggregate;

public partial class FormTests
{
    [Fact]
    public void Clone_ShouldReturnClonedForm_WhenStatusIsPublishedAndLanguagesAreValid()
    {
        // Arrange
        var template = Form.Create(FormType.Opening,
            "A",
            LanguagesList.RO.Iso1,
            new TranslatedStringFaker(_languages),
            new TranslatedStringFaker(_languages),
            _languages,
            [
                _textQuestion,
                _numberQuestion,
                _dateQuestion,
                _singleSelectQuestion,
                _multiSelectQuestion,
                _ratingQuestion
            ]);
        template.Publish();

        var electionRoundId = Guid.NewGuid();
        var monitoringNgoId = Guid.NewGuid();

        // Act
        string[] languages = [LanguagesList.EN.Iso1];
        var form = template.Clone(electionRoundId, monitoringNgoId, LanguagesList.EN.Iso1, languages);

        // Assert
        form.Should().NotBeNull();
        form.Should().NotBeSameAs(template);
        form.ElectionRoundId.Should().Be(electionRoundId);
        form.MonitoringNgoId.Should().Be(monitoringNgoId);
        form.DefaultLanguage.Should().Be(LanguagesList.EN.Iso1);
        form.Languages.Should().BeEquivalentTo(languages);
        form.Name.Should().BeEquivalentTo(template.Name.TrimTranslations(languages));
        form.Description.Should().BeEquivalentTo(template.Description?.TrimTranslations(languages));
        form.Questions.Should().BeEquivalentTo(template.Questions.Select(x => x.TrimTranslations(languages)));
    }

    [Fact]
    public void Clone_ShouldThrowValidationException_WhenStatusIsNotPublished()
    {
        // Arrange
        // Arrange
        var template = Form.Create(FormType.Opening,
            "A",
            LanguagesList.RO.Iso1,
            new TranslatedStringFaker(_languages),
            new TranslatedStringFaker(_languages),
            _languages,
            [
                _textQuestion,
                _numberQuestion,
                _dateQuestion,
                _singleSelectQuestion,
                _multiSelectQuestion,
                _ratingQuestion
            ]);
        var electionRoundId = Guid.NewGuid();
        var monitoringNgoId = Guid.NewGuid();

        // Act
        Action act = () => template.Clone(electionRoundId, monitoringNgoId, LanguagesList.RO.Iso1,
            _languages);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*Form template is not published*")
            .Where(e => e.Errors.Any(f => f.PropertyName == "Status"));
    }

    [Fact]
    public void Clone_ShouldThrowValidationException_WhenDefaultLanguageIsUnsupported()
    {
        // Arrange
        var template = Form.Create(FormType.Opening,
            "A",
            LanguagesList.RO.Iso1,
            new TranslatedStringFaker(_languages),
            new TranslatedStringFaker(_languages),
            _languages,
            [
                _textQuestion,
                _numberQuestion,
                _dateQuestion,
                _singleSelectQuestion,
                _multiSelectQuestion,
                _ratingQuestion
            ]);
        template.Publish();

        var electionRoundId = Guid.NewGuid();
        var monitoringNgoId = Guid.NewGuid();

        // Act
        Action act = () =>
            template.Clone(electionRoundId, monitoringNgoId, LanguagesList.RM.Iso1, [LanguagesList.RO.Iso1]);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*Default language is not supported*")
            .Where(e => e.Errors.Any(f => f.PropertyName == "defaultLanguage"));
    }

    [Fact]
    public void Clone_ShouldThrowValidationException_WhenOneOrMoreLanguagesAreUnsupported()
    {
        // Arrange
        var template = Form.Create(FormType.Opening,
            "A",
            LanguagesList.RO.Iso1,
            new TranslatedStringFaker(_languages),
            new TranslatedStringFaker(_languages),
            _languages,
            [
                _textQuestion,
                _numberQuestion,
                _dateQuestion,
                _singleSelectQuestion,
                _multiSelectQuestion,
                _ratingQuestion
            ]);
        template.Publish();

        var electionRoundId = Guid.NewGuid();
        var monitoringNgoId = Guid.NewGuid();

        // Act
        Action act = () =>
            template.Clone(electionRoundId, monitoringNgoId, LanguagesList.RO.Iso1, [LanguagesList.RM.Iso1]);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*Language is not supported*")
            .Where(e => e.Errors.Any(f => f.PropertyName == "languages.RM"));
    }
}
