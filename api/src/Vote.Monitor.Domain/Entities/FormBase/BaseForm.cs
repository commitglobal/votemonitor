using FluentValidation;
using Newtonsoft.Json;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
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
        LanguagesTranslationStatus =
            AnswersHelpers.ComputeLanguagesTranslationStatus(Questions, defaultLanguage, Languages);
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
        LanguagesTranslationStatus =
            AnswersHelpers.ComputeLanguagesTranslationStatus(Questions, defaultLanguage, Languages);
    }

    private T BaseFillIn<T>(T submission, List<BaseAnswer> answers, Action<T> clearAnswers,
        Action<T, int, int, List<BaseAnswer>> updateAnswers) where T : class
    {
        if (answers == null)
        {
            return submission;
        }

        if (!answers.Any())
        {
            clearAnswers(submission);
            return submission;
        }

        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var numberOfQuestionsAnswered = AnswersHelpers.CountNumberOfQuestionsAnswered(Questions, answers);
        var numberOfFlaggedAnswers = AnswersHelpers.CountNumberOfFlaggedAnswers(Questions, answers);

        updateAnswers(submission, numberOfQuestionsAnswered, numberOfFlaggedAnswers, answers);

        return submission;
    }

    public FormSubmission FillIn(FormSubmission formSubmission, List<BaseAnswer> answers)
    {
        return BaseFillIn(
            formSubmission,
            answers,
            submission => submission.ClearAnswers(),
            (submission, numberOfQuestionsAnswered, numberOfFlaggedAnswers, formAnswers) =>
                submission.UpdateAnswers(numberOfQuestionsAnswered, numberOfFlaggedAnswers, formAnswers)
        );
    }

    public CitizenReport FillIn(CitizenReport citizenReport, List<BaseAnswer> answers)
    {
        return BaseFillIn(
            citizenReport,
            answers,
            report => report.ClearAnswers(),
            (report, numberOfQuestionsAnswered, numberOfFlaggedAnswers, ans) =>
                report.UpdateAnswers(numberOfQuestionsAnswered, numberOfFlaggedAnswers, ans)
        );
    }

    public PollingStationInformation FillIn(PollingStationInformation psiSubmission, List<BaseAnswer> answers)
    {
        return BaseFillIn(
            psiSubmission,
            answers,
            submission => submission.ClearAnswers(),
            (submission, answered, flagged, ans) => submission.UpdateAnswers(ans, answered, flagged)
        );
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

        LanguagesTranslationStatus =
            AnswersHelpers.ComputeLanguagesTranslationStatus(Questions, DefaultLanguage, Languages);
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

        LanguagesTranslationStatus =
            AnswersHelpers.ComputeLanguagesTranslationStatus(Questions, DefaultLanguage, Languages);
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
            AnswersHelpers.ComputeLanguagesTranslationStatus(Questions, DefaultLanguage, Languages);
    }

    protected BaseForm()
    {
    }
}