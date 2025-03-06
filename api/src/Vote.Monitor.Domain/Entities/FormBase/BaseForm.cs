using FluentValidation;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Domain.Entities.FormBase;

public abstract class BaseForm : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public FormType FormType { get; private set; }
    public string Code { get; private set; }
    public TranslatedString Name { get; private set; }
    public TranslatedString Description { get; private set; }
    public FormStatus Status { get; protected set; }
    public string DefaultLanguage { get; private set; }
    public string[] Languages { get; private set; } = [];
    public string? Icon { get; private set; }
    public int NumberOfQuestions { get; private set; }

    public LanguagesTranslationStatus LanguagesTranslationStatus { get; private set; } = new();
    public IReadOnlyList<BaseQuestion> Questions { get; private set; } = new List<BaseQuestion>().AsReadOnly();

    protected BaseForm(
        FormType formType,
        string code,
        TranslatedString name,
        TranslatedString description,
        string defaultLanguage,
        IEnumerable<string> languages,
        string? icon,
        IEnumerable<BaseQuestion> questions,
        FormStatus status)
    {
        Id = Guid.NewGuid();

        FormType = formType;
        Code = code;
        Name = name;
        Description = description;
        DefaultLanguage = defaultLanguage;
        Languages = languages.ToArray();
        Status = status;
        Questions = questions.ToList().AsReadOnly();
        NumberOfQuestions = Questions.Count;
        Icon = icon;
        LanguagesTranslationStatus = ComputeLanguagesTranslationStatus();
    }

    [JsonConstructor]
    public BaseForm(Guid id,
        FormType formType,
        string code,
        TranslatedString name,
        TranslatedString description,
        FormStatus status,
        string defaultLanguage,
        string[] languages,
        string? icon,
        int numberOfQuestions,
        LanguagesTranslationStatus languagesTranslationStatus)
    {
        Id = id;
        FormType = formType;
        Code = code;
        Name = name;
        Description = description;
        Status = status;
        DefaultLanguage = defaultLanguage;
        Languages = languages;
        Icon = icon;
        NumberOfQuestions = numberOfQuestions;
        LanguagesTranslationStatus = languagesTranslationStatus;
    }

    public DraftFormResult Draft()
    {
        var draftInternalResult = DraftInternal();
        if (draftInternalResult is not DraftFormResult.Drafted)
        {
            return draftInternalResult;
        }

        Status = FormStatus.Drafted;
        return new DraftFormResult.Drafted();
    }

    public abstract DraftFormResult DraftInternal();

    public ObsoleteFormResult Obsolete()
    {
        var obsoleteInternalResult = ObsoleteInternal();
        if (obsoleteInternalResult is not ObsoleteFormResult.Obsoleted)
        {
            return obsoleteInternalResult;
        }

        Status = FormStatus.Obsolete;
        return new ObsoleteFormResult.Obsoleted();
    }

    public abstract ObsoleteFormResult ObsoleteInternal();

    public PublishFormResult Publish()
    {
        var validator = new BaseFormValidator();
        var validationResult = validator.Validate(this);

        if (!validationResult.IsValid)
        {
            return new PublishFormResult.Error(validationResult);
        }

        var publishInternalResult = PublishInternal();
        if (publishInternalResult is not PublishFormResult.Published)
        {
            return publishInternalResult;
        }

        Status = FormStatus.Published;
        return new PublishFormResult.Published();
    }

    public abstract PublishFormResult PublishInternal();


    public void UpdateDetails(string code,
        TranslatedString name,
        TranslatedString description,
        FormType formType,
        string defaultLanguage,
        IEnumerable<string> languages,
        string? icon,
        IEnumerable<BaseQuestion> questions)
    {
        Code = code;
        Name = name;
        Description = description;
        FormType = formType;
        DefaultLanguage = defaultLanguage;
        Languages = languages.ToArray();
        Questions = questions.ToList().AsReadOnly();
        NumberOfQuestions = Questions.Count;
        Icon = icon;
        LanguagesTranslationStatus = ComputeLanguagesTranslationStatus();
    }

    private void ValidateAnswers(List<BaseAnswer>? answers)
    {
        var validationResult = AnswersValidator.GetValidationResults(answers ?? [], Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
    }

    private (int? numberOfQuestionsAnswered, int? numberOfFlaggedAnswers) CalculateAnswerMetrics(
        List<BaseAnswer>? answers)
    {
        if (answers is null)
        {
            return (null, null);
        }

        var numberOfQuestionsAnswered = AnswersHelpers.CountNumberOfQuestionsAnswered(Questions, answers);
        var numberOfFlaggedAnswers = AnswersHelpers.CountNumberOfFlaggedAnswers(Questions, answers);

        return (numberOfQuestionsAnswered, numberOfFlaggedAnswers);
    }

    public FormSubmission FillIn(FormSubmission formSubmission, List<BaseAnswer>? answers, bool? isCompleted,
        DateTime lastUpdatedAt)
    {
        ValidateAnswers(answers);
        var (numberOfQuestionsAnswered, numberOfFlaggedAnswers) = CalculateAnswerMetrics(answers);

        formSubmission.Update(answers, numberOfQuestionsAnswered, numberOfFlaggedAnswers, isCompleted, lastUpdatedAt);

        return formSubmission;
    }

    public CitizenReport FillIn(CitizenReport citizenReport, List<BaseAnswer>? answers)
    {
        ValidateAnswers(answers);
        var (numberOfQuestionsAnswered, numberOfFlaggedAnswers) = CalculateAnswerMetrics(answers);

        citizenReport.Update(answers, numberOfQuestionsAnswered, numberOfFlaggedAnswers);

        return citizenReport;
    }

    public PollingStationInformation FillIn(PollingStationInformation psiSubmission,
        List<BaseAnswer>? answers,
        ValueOrUndefined<DateTime?> arrivalTime,
        ValueOrUndefined<DateTime?> departureTime,
        List<ObservationBreak>? breaks,
        ValueOrUndefined<bool> isCompleted,
        DateTime lastUpdatedAt)
    {
        ValidateAnswers(answers);
        var (numberOfQuestionsAnswered, numberOfFlaggedAnswers) = CalculateAnswerMetrics(answers);

        psiSubmission.Update(answers, numberOfQuestionsAnswered, numberOfFlaggedAnswers, arrivalTime,
            departureTime, breaks, isCompleted, lastUpdatedAt);

        return psiSubmission;
    }

    public IncidentReport FillIn(IncidentReport incidentReport, List<BaseAnswer>? answers, bool? isCompleted,
        DateTime lastUpdatedAt)
    {
        ValidateAnswers(answers);
        var (numberOfQuestionsAnswered, numberOfFlaggedAnswers) = CalculateAnswerMetrics(answers);

        incidentReport.Update(answers, numberOfQuestionsAnswered, numberOfFlaggedAnswers, isCompleted, lastUpdatedAt);

        return incidentReport;
    }

    public void AddTranslations(string[] languageCodes)
    {
        var newLanguages = languageCodes.Except(Languages);
        Languages = Languages.Union(languageCodes).ToArray();

        foreach (var languageCode in newLanguages)
        {
            Description.AddTranslation(languageCode);
            Name.AddTranslation(languageCode);

            foreach (var question in Questions)
            {
                question.AddTranslation(languageCode);
            }
        }

        LanguagesTranslationStatus = ComputeLanguagesTranslationStatus();
    }

    public bool HasTranslation(string languageCode)
    {
        return Languages.Contains(languageCode);
    }

    public void SetDefaultLanguage(string languageCode)
    {
        if (!HasTranslation(languageCode))
        {
            throw new ArgumentException("Form does not have translations for language code");
        }

        DefaultLanguage = languageCode;

        LanguagesTranslationStatus = ComputeLanguagesTranslationStatus();
    }

    public void RemoveTranslation(string languageCode)
    {
        bool hasLanguageCode = languageCode.Contains(languageCode);

        if (!hasLanguageCode)
        {
            return;
        }

        if (DefaultLanguage == languageCode)
        {
            throw new ArgumentException("Cannot remove default language");
        }

        Languages = Languages.Except([languageCode]).ToArray();
        Description.RemoveTranslation(languageCode);
        Name.RemoveTranslation(languageCode);

        foreach (var question in Questions)
        {
            question.RemoveTranslation(languageCode);
        }

        LanguagesTranslationStatus =
            ComputeLanguagesTranslationStatus();
    }

    private LanguagesTranslationStatus ComputeLanguagesTranslationStatus()
    {
        var languagesTranslationStatus = new LanguagesTranslationStatus();

        foreach (var languageCode in Languages)
        {
            if (Name != null && (!Name.ContainsKey(languageCode) || string.IsNullOrWhiteSpace(Name[languageCode])))
            {
                languagesTranslationStatus.AddOrUpdateTranslationStatus(languageCode,
                    TranslationStatus.MissingTranslations);
                continue;
            }

            if (Description != null)
            {
                if (Description.ContainsKey(DefaultLanguage) &&
                    !string.IsNullOrWhiteSpace(Description[DefaultLanguage]) &&
                    (!Description.ContainsKey(languageCode) ||
                     string.IsNullOrWhiteSpace(Description[languageCode])))
                {
                    languagesTranslationStatus.AddOrUpdateTranslationStatus(languageCode,
                        TranslationStatus.MissingTranslations);
                    continue;
                }
            }

            var status =
                Questions.Any(x =>
                    x.GetTranslationStatus(DefaultLanguage, languageCode) == TranslationStatus.MissingTranslations)
                    ? TranslationStatus.MissingTranslations
                    : TranslationStatus.Translated;

            languagesTranslationStatus.AddOrUpdateTranslationStatus(languageCode, status);
        }

        return languagesTranslationStatus;
    }

    protected BaseForm()
    {
    }
}
