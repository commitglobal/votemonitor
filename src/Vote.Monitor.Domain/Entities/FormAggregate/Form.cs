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

    public IReadOnlyList<string> Languages { get; private set; } = new List<string>().AsReadOnly();

    public IReadOnlyList<BaseQuestion> Questions { get; private set; } = new List<BaseQuestion>().AsReadOnly();

    private Form(
        ElectionRound electionRound,
        MonitoringNgo monitoringNgo,
        FormType formType,
        string code,
        TranslatedString name,
        IEnumerable<string> languages) : base(Guid.NewGuid())
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        MonitoringNgo = monitoringNgo;
        MonitoringNgoId = monitoringNgo.Id;

        FormType = formType;
        Code = code;
        Name = name;
        Languages = languages.ToList().AsReadOnly();
        Status = FormStatus.Drafted;
    }

    public static Form Create(
        ElectionRound electionRound,
        MonitoringNgo monitoringNgo,
        FormType formType,
        string code,
        TranslatedString name,
        IEnumerable<string> languages) =>
        new(electionRound, monitoringNgo, formType, code, name, languages);

    public PublishResult Publish()
    {
        var validator = new FormValidator();
        var validationResult = validator.Validate(this);

        if (!validationResult.IsValid)
        {
            return new PublishResult.InvalidFormTemplate(validationResult);
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
        IEnumerable<string> languages,
        IEnumerable<BaseQuestion> questions)
    {
        Code = code;
        Name = name;
        FormType = formType;
        Languages = languages.ToList().AsReadOnly();
        Questions = questions.ToList().AsReadOnly();
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private Form()
    {

    }
#pragma warning restore CS8618
}
