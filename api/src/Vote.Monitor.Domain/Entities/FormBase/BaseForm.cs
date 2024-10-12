using FluentValidation;
using Newtonsoft.Json;
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

public class BaseForm : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; set; }
    public ElectionRound ElectionRound { get; set; }
    public FormType FormType { get; private set; }
    public string Code { get; private set; }
    public TranslatedString Name { get; private set; }
    public TranslatedString Description { get; private set; }
    public FormStatus Status { get; private set; }
    public string DefaultLanguage { get; private set; }
    public string[] Languages { get; private set; } = [];
    public int NumberOfQuestions { get; private set; }

    public LanguagesTranslationStatus LanguagesTranslationStatus { get; private set; } = new();
    public IReadOnlyList<BaseQuestion> Questions { get; private set; } = new List<BaseQuestion>().AsReadOnly();

    protected BaseForm(
        ElectionRound electionRound,
        FormType formType,
        string code,
        TranslatedString name,
        TranslatedString description,
        string defaultLanguage,
        IEnumerable<string> languages,
        IEnumerable<BaseQuestion> questions) : this(electionRound.Id, formType, code, name,
        description, defaultLanguage, languages, questions)
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
    }

    protected BaseForm(
        Guid electionRoundId,
        FormType formType,
        string code,
        TranslatedString name,
        TranslatedString description,
        string defaultLanguage,
        IEnumerable<string> languages,
        IEnumerable<BaseQuestion> questions) : base(Guid.NewGuid())
    {
        ElectionRoundId = electionRoundId;

        FormType = formType;
        Code = code;
        Name = name;
        Description = description;
        DefaultLanguage = defaultLanguage;
        Languages = languages.ToArray();
        Status = FormStatus.Drafted;
        Questions = questions.ToList().AsReadOnly();
        NumberOfQuestions = Questions.Count;
        LanguagesTranslationStatus = ComputeLanguagesTranslationStatus();
    }

    [JsonConstructor]
    public BaseForm(Guid id,
        Guid electionRoundId,
        FormType formType,
        string code,
        TranslatedString name,
        TranslatedString description,
        FormStatus status,
        string defaultLanguage,
        string[] languages,
        int numberOfQuestions,
        LanguagesTranslationStatus languagesTranslationStatus) : base(id)
    {
        ElectionRoundId = electionRoundId;
        FormType = formType;
        Code = code;
        Name = name;
        Description = description;
        Status = status;
        DefaultLanguage = defaultLanguage;
        Languages = languages;
        NumberOfQuestions = numberOfQuestions;
        LanguagesTranslationStatus = languagesTranslationStatus;
    }

    public PublishResult Publish()
    {
        var validator = new FormValidator();
        var validationResult = validator.Validate(this);

        if (!validationResult.IsValid)
        {
            return new PublishResult.InvalidForm(validationResult);
        }

        Status = FormStatus.Published;

        return new PublishResult.Published();
    }

    public void Draft()
    {
        Status = FormStatus.Drafted;
    }

    public void Obsolete()
    {
        Status = FormStatus.Obsolete;
    }

    public void UpdateDetails(string code,
        TranslatedString name,
        TranslatedString description,
        FormType formType,
        string defaultLanguage,
        IEnumerable<string> languages,
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

    public FormSubmission FillIn(FormSubmission formSubmission, List<BaseAnswer>? answers, bool? isCompleted)
    {
        ValidateAnswers(answers);
        var (numberOfQuestionsAnswered, numberOfFlaggedAnswers) = CalculateAnswerMetrics(answers);

        formSubmission.Update(answers, numberOfQuestionsAnswered, numberOfFlaggedAnswers, isCompleted);

        return formSubmission;
    }

    public CitizenReport FillIn(CitizenReport citizenReport, List<BaseAnswer>? answers)
    {
        ValidateAnswers(answers);
        var (numberOfQuestionsAnswered, numberOfFlaggedAnswers) = CalculateAnswerMetrics(answers);

        citizenReport.Update(answers, numberOfQuestionsAnswered, numberOfFlaggedAnswers);

        return citizenReport;
    }

    public PollingStationInformation FillIn(PollingStationInformation psiSubmission, List<BaseAnswer>? answers,
        DateTime? arrivalTime, DateTime? departureTime, List<ObservationBreak>? breaks, bool? isCompleted)
    {
        ValidateAnswers(answers);
        var (numberOfQuestionsAnswered, numberOfFlaggedAnswers) = CalculateAnswerMetrics(answers);

        psiSubmission.Update(answers, numberOfQuestionsAnswered, numberOfFlaggedAnswers, arrivalTime,
            departureTime, breaks, isCompleted);

        return psiSubmission;
    }
    
    public IncidentReport FillIn(IncidentReport incidentReport, List<BaseAnswer>? answers, bool? isCompleted)
    {
        ValidateAnswers(answers);
        var (numberOfQuestionsAnswered, numberOfFlaggedAnswers) = CalculateAnswerMetrics(answers);

        incidentReport.Update(answers, numberOfQuestionsAnswered, numberOfFlaggedAnswers, isCompleted);

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