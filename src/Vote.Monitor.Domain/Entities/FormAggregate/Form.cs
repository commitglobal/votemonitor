using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

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

#pragma warning disable CS8618 // Required by Entity Framework
    private Form()
    {

    }
#pragma warning restore CS8618
}
