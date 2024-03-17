using Vote.Monitor.Domain.Entities.FormAnswerBase;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

public class PollingStationInformationForm : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public PollingStationInfoFormStatus Status { get; private set; }
    public IReadOnlyList<string> Languages { get; private set; } = new List<string>().AsReadOnly();
    public IReadOnlyList<BaseQuestion> Questions { get; private set; } = new List<BaseQuestion>().AsReadOnly();

    private PollingStationInformationForm(
        ElectionRound electionRound,
        IEnumerable<string> languages,
        ITimeProvider timeProvider) : base(Guid.NewGuid(), timeProvider)
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        Languages = languages.ToList().AsReadOnly();
        Status = PollingStationInfoFormStatus.Drafted;
    }

    public static PollingStationInformationForm Create(
        ElectionRound electionRound,
        IEnumerable<string> languages,
        ITimeProvider timeProvider) =>
        new(electionRound, languages, timeProvider);

    public PublishResult Publish()
    {
        var validator = new PollingStationInfoFormValidator();
        var validationResult = validator.Validate(this);

        if (!validationResult.IsValid)
        {
            return new PublishResult.InvalidFormTemplate(validationResult);
        }

        Status = PollingStationInfoFormStatus.Published;
        return new PublishResult.Published();
    }

    public void UpdateDetails(IEnumerable<string> languages, IEnumerable<BaseQuestion> questions)
    {
        Languages = languages.ToList().AsReadOnly();
        Questions = questions.ToList().AsReadOnly();
    }

    public PollingStationInformation CreatePollingStationInformation(PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        ITimeProvider timeProvider)
    {
        return PollingStationInformation.Create(ElectionRound, pollingStation, monitoringObserver, this, timeProvider);
    }

    public FillInPollingStationInformationResult FillIn(
        PollingStationInformation filledInForm,
        List<BaseAnswer> answers)
    {
        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            return new FillInPollingStationInformationResult.ValidationFailed(validationResult);
        }

        filledInForm.UpdateDetails(answers);

        return new FillInPollingStationInformationResult.Ok(filledInForm);
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private PollingStationInformationForm()
    {

    }
#pragma warning restore CS8618
}
