using FluentValidation;
using Vote.Monitor.Core.Models;
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
    public FormStatus Status { get; private set; }
    public string DefaultLanguage { get; private set; }
    public string[] Languages { get; private set; } = [];

    public IReadOnlyList<BaseQuestion> Questions { get; private set; } = new List<BaseQuestion>().AsReadOnly();

    private Form(
        ElectionRound electionRound,
        MonitoringNgo monitoringNgo,
        FormType formType,
        string code,
        TranslatedString name,
        string defaultLanguage,
        IEnumerable<string> languages,
        IEnumerable<BaseQuestion> questions) : base(Guid.NewGuid())
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        MonitoringNgo = monitoringNgo;
        MonitoringNgoId = monitoringNgo.Id;

        FormType = formType;
        Code = code;
        Name = name;
        DefaultLanguage = defaultLanguage;
        Languages = languages.ToArray();
        Status = FormStatus.Drafted;
        Questions = questions.ToList().AsReadOnly();
    }
    private Form(
        Guid electionRoundId,
        Guid monitoringNgoId,
        FormType formType,
        string code,
        TranslatedString name,
        string defaultLanguage,
        IEnumerable<string> languages,
        IEnumerable<BaseQuestion> questions) : base(Guid.NewGuid())
    {
        ElectionRoundId = electionRoundId;
        MonitoringNgoId = monitoringNgoId;

        FormType = formType;
        Code = code;
        Name = name;
        DefaultLanguage = defaultLanguage;
        Languages = languages.ToArray();
        Status = FormStatus.Drafted;
        Questions = questions.ToList().AsReadOnly();
    }

    public static Form Create(
        ElectionRound electionRound,
        MonitoringNgo monitoringNgo,
        FormType formType,
        string code,
        TranslatedString name,
        string defaultLanguage,
        IEnumerable<string> languages,
        IEnumerable<BaseQuestion> questions) =>
        new(electionRound, monitoringNgo, formType, code, name, defaultLanguage, languages, questions);

    public static Form Create(
        Guid electionRoundId,
        Guid monitoringNgoId,
        FormType formType,
        string code,
        TranslatedString name,
        string defaultLanguage,
        IEnumerable<string> languages,
        IEnumerable<BaseQuestion> questions) =>
        new(electionRoundId, monitoringNgoId, formType, code, name, defaultLanguage, languages, questions);

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
        FormType formType,
        string defaultLanguage,
        IEnumerable<string> languages,
        IEnumerable<BaseQuestion> questions)
    {
        Code = code;
        Name = name;
        FormType = formType;
        DefaultLanguage = defaultLanguage;
        Languages = languages.ToArray();
        Questions = questions.ToList().AsReadOnly();
    }

    public FormSubmission CreateFormSubmission(PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        List<BaseAnswer> answers)
    {
        var numberOfQuestionAnswered = CountNumberOfQuestionsAnswered(answers);
        var numberOfFlaggedAnswers = CountNumberOfFlaggedAnswers(answers);

        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        return FormSubmission.Create(ElectionRound, pollingStation, monitoringObserver, this, answers, numberOfQuestionAnswered, numberOfFlaggedAnswers);
    }

    private int CountNumberOfFlaggedAnswers(List<BaseAnswer> answers)
    {
        var singleSelectFlaggedOptionIds =
            Questions
                .OfType<SingleSelectQuestion>()
                .SelectMany(x => x.Options)
                .Where(x => x.IsFlagged)
                .Select(x => x.Id)
                .ToList();

        var multiSelectFlaggedOptionIds =
            Questions
                .OfType<MultiSelectQuestion>()
                .SelectMany(x => x.Options)
                .Where(x => x.IsFlagged)
                .Select(x => x.Id)
                .ToList();

        return answers
                   .OfType<SingleSelectAnswer>()
                   .Count(x => singleSelectFlaggedOptionIds.Contains(x.Selection.OptionId))
               + answers
                   .OfType<MultiSelectAnswer>()
                   .SelectMany(x => x.Selection)
                   .Count(x => multiSelectFlaggedOptionIds.Contains(x.OptionId));
    }

    private int CountNumberOfQuestionsAnswered(List<BaseAnswer> answers)
    {
        var questionIds = Questions.Select(x => x.Id).ToList();

        return answers.Count(x => questionIds.Contains(x.QuestionId));
    }

    public FormSubmission FillIn(FormSubmission submission, List<BaseAnswer> answers)
    {
        if (!answers.Any())
        {
            return submission;
        }

        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var numberOfQuestionAnswered = CountNumberOfQuestionsAnswered(answers);
        var numberOfFlaggedAnswers = CountNumberOfFlaggedAnswers(answers);

        submission.UpdateAnswers(numberOfQuestionAnswered, numberOfFlaggedAnswers, answers);

        return submission;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private Form()
    {

    }
#pragma warning restore CS8618
}
