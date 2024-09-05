using System.Text.Json.Serialization;
using FluentValidation;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormAnswerBase;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.FormAggregate;

public class Form : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; set; }
    public ElectionRound ElectionRound { get; set; }
    public Guid MonitoringNgoId { get; set; }
    public MonitoringNgo MonitoringNgo { get; set; }
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

    private Form(
        ElectionRound electionRound,
        MonitoringNgo monitoringNgo,
        FormType formType,
        string code,
        TranslatedString name,
        TranslatedString description,
        string defaultLanguage,
        IEnumerable<string> languages,
        IEnumerable<BaseQuestion> questions) : this(electionRound.Id, monitoringNgo.Id, formType, code, name,
        description, defaultLanguage, languages, questions)
    {
        ElectionRound = electionRound;
        MonitoringNgo = monitoringNgo;
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
        IEnumerable<BaseQuestion> questions) : base(Guid.NewGuid())
    {
        ElectionRoundId = electionRoundId;
        MonitoringNgoId = monitoringNgoId;

        FormType = formType;
        Code = code;
        Name = name;
        Description = description;
        DefaultLanguage = defaultLanguage;
        Languages = languages.ToArray();
        Status = FormStatus.Drafted;
        Questions = questions.ToList().AsReadOnly();
        NumberOfQuestions = Questions.Count;
        LanguagesTranslationStatus = ComputeLanguagesTranslationStatus(Questions, defaultLanguage, Languages);
    }

    private LanguagesTranslationStatus ComputeLanguagesTranslationStatus(IReadOnlyList<BaseQuestion> questions,
        string defaultLanguage, string[] languages)
    {
        var languagesTranslationStatus = new LanguagesTranslationStatus();

        foreach (var languageCode in languages)
        {
            var status =
                Questions.Any(x =>
                    x.GetTranslationStatus(defaultLanguage, languageCode) == TranslationStatus.MissingTranslations)
                    ? TranslationStatus.MissingTranslations
                    : TranslationStatus.Translated;

            languagesTranslationStatus.AddOrUpdateTranslationStatus(languageCode, status);
        }

        return languagesTranslationStatus;
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
        int numberOfQuestions,
        LanguagesTranslationStatus languagesTranslationStatus) : base(id)
    {
        ElectionRoundId = electionRoundId;
        MonitoringNgoId = monitoringNgoId;
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

    public static Form Create(
        ElectionRound electionRound,
        MonitoringNgo monitoringNgo,
        FormType formType,
        string code,
        TranslatedString name,
        TranslatedString description,
        string defaultLanguage,
        IEnumerable<string> languages,
        IEnumerable<BaseQuestion> questions) =>
        new(electionRound, monitoringNgo, formType, code, name, description, defaultLanguage, languages, questions);

    public static Form Create(
        Guid electionRoundId,
        Guid monitoringNgoId,
        FormType formType,
        string code,
        TranslatedString name,
        TranslatedString description,
        string defaultLanguage,
        IEnumerable<string> languages,
        IEnumerable<BaseQuestion> questions) =>
        new(electionRoundId, monitoringNgoId, formType, code, name, description, defaultLanguage, languages, questions);

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
        LanguagesTranslationStatus = ComputeLanguagesTranslationStatus(Questions, defaultLanguage, Languages);
    }

    public FormSubmission CreateFormSubmission(
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        List<BaseAnswer>? answers)
    {
        if (answers == null)
        {
            return FormSubmission.Create(ElectionRound, pollingStation, monitoringObserver, this, [], 0, 0);
        }

        var numberOfQuestionAnswered = CountNumberOfQuestionsAnswered(answers);
        var numberOfFlaggedAnswers = CountNumberOfFlaggedAnswers(answers);

        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        return FormSubmission.Create(ElectionRound, pollingStation, monitoringObserver, this, answers,
            numberOfQuestionAnswered, numberOfFlaggedAnswers);
    }

    public CitizenReport CreateCitizenReport(Guid citizenReportId, List<BaseAnswer>? answers)
    {
        if (answers == null)
        {
            return CitizenReport.Create(citizenReportId, ElectionRound, this, [], 0, 0);
        }

        var numberOfQuestionAnswered = CountNumberOfQuestionsAnswered(answers);
        var numberOfFlaggedAnswers = CountNumberOfFlaggedAnswers(answers);

        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        return CitizenReport.Create(citizenReportId, ElectionRound, this, answers, numberOfQuestionAnswered, numberOfFlaggedAnswers);
    }

    private int CountNumberOfFlaggedAnswers(List<BaseAnswer> answers)
    {
        var singleSelectQuestions =
            Questions
                .OfType<SingleSelectQuestion>()
                .ToList();

        var multiSelectQuestions =
            Questions
                .OfType<MultiSelectQuestion>()
                .ToList();

        int flaggedAnswers = 0;
        foreach (var singleSelectAnswer in answers.OfType<SingleSelectAnswer>())
        {
            var option = singleSelectQuestions
                .FirstOrDefault(x => x.Id == singleSelectAnswer.QuestionId)
                ?.Options
                ?.FirstOrDefault(x => x.Id == singleSelectAnswer.Selection.OptionId);

            // Just in case 
            if (option is null)
            {
                continue;
            }

            if (option.IsFlagged)
            {
                flaggedAnswers++;
            }
        }

        foreach (var multiSelectAnswer in answers.OfType<MultiSelectAnswer>())
        {
            var options = multiSelectQuestions
                .FirstOrDefault(x => x.Id == multiSelectAnswer.QuestionId)
                ?.Options
                ?.Where(x => multiSelectAnswer.Selection.Select(x => x.OptionId).Contains(x.Id));

            // Just in case 
            if (options is null)
            {
                continue;
            }

            flaggedAnswers += options.Count(x => x.IsFlagged);
        }

        return flaggedAnswers;
    }

    private int CountNumberOfQuestionsAnswered(List<BaseAnswer> answers)
    {
        var questionIds = Questions.Select(x => x.Id).ToList();

        return answers.Count(x => questionIds.Contains(x.QuestionId));
    }

    public FormSubmission FillIn(FormSubmission submission, List<BaseAnswer>? answers)
    {
        if (answers == null)
        {
            return submission;
        }

        if (!answers.Any())
        {
            submission.ClearAnswers();
            return submission;
        }

        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var numberOfQuestionsAnswered = CountNumberOfQuestionsAnswered(answers);
        var numberOfFlaggedAnswers = CountNumberOfFlaggedAnswers(answers);

        submission.UpdateAnswers(numberOfQuestionsAnswered, numberOfFlaggedAnswers, answers);

        return submission;
    }
    public CitizenReport FillIn(CitizenReport citizenReport, List<BaseAnswer>? answers)
    {
        if (answers == null)
        {
            return citizenReport;
        }

        if (!answers.Any())
        {
            citizenReport.ClearAnswers();
            return citizenReport;
        }

        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var numberOfQuestionsAnswered = CountNumberOfQuestionsAnswered(answers);
        var numberOfFlaggedAnswers = CountNumberOfFlaggedAnswers(answers);

        citizenReport.UpdateAnswers(numberOfQuestionsAnswered, numberOfFlaggedAnswers, answers);

        return citizenReport;
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

        LanguagesTranslationStatus = ComputeLanguagesTranslationStatus(Questions, DefaultLanguage, Languages);
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

        LanguagesTranslationStatus = ComputeLanguagesTranslationStatus(Questions, DefaultLanguage, Languages);
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

        LanguagesTranslationStatus = ComputeLanguagesTranslationStatus(Questions, DefaultLanguage, Languages);
    }

  
    public Form Duplicate() =>
        new(ElectionRoundId, MonitoringNgoId, FormType, Code, Name, Description, DefaultLanguage, Languages, Questions);

#pragma warning disable CS8618 // Required by Entity Framework
    private Form()
    {
    }
#pragma warning restore CS8618
}