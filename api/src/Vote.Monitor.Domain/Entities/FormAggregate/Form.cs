using FluentValidation;
using FluentValidation.Results;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.LocationAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.FormAggregate;

public class Form : BaseForm
{
    public Guid MonitoringNgoId { get; private set; }
    public MonitoringNgo MonitoringNgo { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }

    private Form(
        ElectionRound electionRound,
        MonitoringNgo monitoringNgo,
        FormType formType,
        string code,
        TranslatedString name,
        TranslatedString description,
        string defaultLanguage,
        IEnumerable<string> languages,
        string? icon,
        int displayOrder,
        IEnumerable<BaseQuestion> questions) : base(
        formType,
        code,
        name,
        description,
        defaultLanguage,
        languages,
        icon,
        questions,
        FormStatus.Drafted,
        displayOrder)
    {
        MonitoringNgoId = monitoringNgo.Id;
        MonitoringNgo = monitoringNgo;
        ElectionRoundId = electionRound.Id;
        ElectionRound = electionRound;
    }

    private Form(
        Guid electionRoundId,
        Guid monitoringNgoId,
        FormType formType,
        string code,
        TranslatedString name,
        TranslatedString description,
        string defaultLanguage,
        IEnumerable<string> languages,
        string? icon,
        int displayOrder,
        IEnumerable<BaseQuestion> questions) : base(
        formType,
        code,
        name,
        description,
        defaultLanguage,
        languages,
        icon,
        questions,
        FormStatus.Drafted,
        displayOrder)
    {
        MonitoringNgoId = monitoringNgoId;
        ElectionRoundId = electionRoundId;
    }

    [JsonConstructor]
    public Form(Guid id,
        Guid electionRoundId,
        Guid monitoringNgoId,
        FormType formType,
        string code,
        TranslatedString name,
        TranslatedString description,
        FormStatus status,
        string defaultLanguage,
        string[] languages,
        string? icon,
        int numberOfQuestions,
        LanguagesTranslationStatus languagesTranslationStatus,
        int displayOrder) : base(id,
        formType,
        code,
        name,
        description,
        status,
        defaultLanguage,
        languages,
        icon,
        numberOfQuestions,
        languagesTranslationStatus,
        displayOrder)
    {
        MonitoringNgoId = monitoringNgoId;
        ElectionRoundId = electionRoundId;
    }

    public static Form Create(
        ElectionRound electionRound,
        MonitoringNgo monitoringNgo,
        FormType formType,
        string code,
        TranslatedString name,
        TranslatedString description,
        string defaultLanguage,
        IEnumerable<string> languages,
        string? icon,
        int displayOrder,
        IEnumerable<BaseQuestion> questions) =>
        new(electionRound, monitoringNgo, formType, code, name, description, defaultLanguage, languages, icon,
            displayOrder, questions);

    public static Form Create(
        Guid electionRoundId,
        Guid monitoringNgoId,
        FormType formType,
        string code,
        TranslatedString name,
        TranslatedString description,
        string defaultLanguage,
        IEnumerable<string> languages,
        string? icon,
        int displayOrder,
        IEnumerable<BaseQuestion> questions) =>
        new(electionRoundId, monitoringNgoId, formType, code, name, description, defaultLanguage, languages, icon,
            displayOrder, questions);

    public Form Duplicate() =>
        new(ElectionRoundId, MonitoringNgoId, FormType, Code, Name, Description, DefaultLanguage, Languages, Icon, DisplayOrder, Questions);

    public Form Clone(Guid electionRoundId, Guid monitoringNgoId, string defaultLanguage, string[] languages)
    {
        if (Status != FormStatus.Published)
        {
            throw new ValidationException([
                new ValidationFailure(nameof(Status), "Form is not published.")
            ]);
        }

        if (!Languages.Contains(defaultLanguage))
        {
            throw new ValidationException([
                new ValidationFailure(nameof(defaultLanguage), "Default language is not supported.")
            ]);
        }

        foreach (var iso in languages)
        {
            if (!Languages.Contains(iso))
            {
                throw new ValidationException([
                    new ValidationFailure(nameof(languages) + $".{iso}", "Language is not supported.")
                ]);
            }
        }

        return Form.Create(electionRoundId,
            monitoringNgoId,
            FormType,
            Code,
            new TranslatedString(Name).TrimTranslations(languages),
            new TranslatedString(Description).TrimTranslations(languages),
            defaultLanguage,
            languages,
            Icon,
            Questions.Select(x => x.DeepClone().TrimTranslations(languages)).ToList());
    }

    public Form Clone(Guid electionRoundId, Guid monitoringNgoId, string defaultLanguage, string[] languages)
    {
        if (Status != FormStatus.Published)
        {
            throw new ValidationException([
                new ValidationFailure(nameof(Status), "Form is not published.")
            ]);
        }

        if (!Languages.Contains(defaultLanguage))
        {
            throw new ValidationException([
                new ValidationFailure(nameof(defaultLanguage), "Default language is not supported.")
            ]);
        }

        foreach (var iso in languages)
        {
            if (!Languages.Contains(iso))
            {
                throw new ValidationException([
                    new ValidationFailure(nameof(languages) + $".{iso}", "Language is not supported.")
                ]);
            }
        }

        return Form.Create(electionRoundId,
            monitoringNgoId,
            FormType,
            Code,
            new TranslatedString(Name).TrimTranslations(languages),
            new TranslatedString(Description).TrimTranslations(languages),
            defaultLanguage,
            languages,
            Icon,
            Questions.Select(x => x.DeepClone().TrimTranslations(languages)).ToList());
    }

    public FormSubmission CreateFormSubmission(
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        List<BaseAnswer>? answers,
        bool? isCompleted)
    {
        answers ??= [];
        var numberOfQuestionAnswered = AnswersHelpers.CountNumberOfQuestionsAnswered(Questions, answers);
        var numberOfFlaggedAnswers = AnswersHelpers.CountNumberOfFlaggedAnswers(Questions, answers);

        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        return FormSubmission.Create(ElectionRound, pollingStation, monitoringObserver, this, answers,
            numberOfQuestionAnswered, numberOfFlaggedAnswers, isCompleted);
    }

    public CitizenReport CreateCitizenReport(Guid citizenReportId, Location location, List<BaseAnswer>? answers)
    {
        answers ??= [];

        var numberOfQuestionAnswered = AnswersHelpers.CountNumberOfQuestionsAnswered(Questions, answers);
        var numberOfFlaggedAnswers = AnswersHelpers.CountNumberOfFlaggedAnswers(Questions, answers);

        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        return CitizenReport.Create(citizenReportId, ElectionRoundId, this, location, answers, numberOfQuestionAnswered,
            numberOfFlaggedAnswers);
    }

    public IncidentReport CreateIncidentReport(
        Guid incidentReportId,
        MonitoringObserver monitoringObserver,
        IncidentReportLocationType locationType,
        string? locationDescription,
        Guid? pollingStationId,
        List<BaseAnswer>? answers,
        bool? isCompleted)
    {
        answers ??= [];

        var numberOfQuestionAnswered = AnswersHelpers.CountNumberOfQuestionsAnswered(Questions, answers);
        var numberOfFlaggedAnswers = AnswersHelpers.CountNumberOfFlaggedAnswers(Questions, answers);

        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        return IncidentReport.Create(incidentReportId, ElectionRoundId, monitoringObserver, locationType,
            pollingStationId,
            locationDescription, Id, answers,
            numberOfQuestionAnswered, numberOfFlaggedAnswers, isCompleted);
    }

    public FormPublishResult Publish()
    {
        var validator = new FormValidator();
        var validationResult = validator.Validate(this);

        if (!validationResult.IsValid)
        {
            return new FormPublishResult.InvalidForm(validationResult);
        }

        Status = FormStatus.Published;

        return new FormPublishResult.Published();
    }


#pragma warning disable CS8618 // Required by Entity Framework
    private Form() : base()
    {
    }
#pragma warning restore CS8618
}
