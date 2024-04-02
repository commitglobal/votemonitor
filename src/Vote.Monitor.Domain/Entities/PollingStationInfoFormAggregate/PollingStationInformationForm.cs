using FluentValidation;
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
    public IReadOnlyList<string> Languages { get; private set; } = new List<string>().AsReadOnly();
    public IReadOnlyList<BaseQuestion> Questions { get; private set; } = new List<BaseQuestion>().AsReadOnly();

    private PollingStationInformationForm(
        ElectionRound electionRound,
        IEnumerable<string> languages,
        List<BaseQuestion> questions) : base(Guid.NewGuid())
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        Languages = languages.ToList().AsReadOnly();
        Questions = questions.ToList().AsReadOnly();
    }
    private PollingStationInformationForm(
        ElectionRound electionRound,
        IEnumerable<string> languages) : this(electionRound, languages, [])
    {
    }

    public static PollingStationInformationForm Create(
        ElectionRound electionRound,
        IEnumerable<string> languages) =>
        new(electionRound, languages);

    public static PollingStationInformationForm Create(
        ElectionRound electionRound,
        IEnumerable<string> languages,
        List<BaseQuestion> questions) =>
        new(electionRound, languages, questions);

    public void UpdateDetails(IEnumerable<string> languages, IEnumerable<BaseQuestion> questions)
    {
        Languages = languages.ToList().AsReadOnly();
        Questions = questions.ToList().AsReadOnly();
    }

    public PollingStationInformation CreatePollingStationInformation(PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        DateTime? arrivalTime,
        DateTime? departureTime,
        List<BaseAnswer> answers)
    {
        return PollingStationInformation.Create(ElectionRound, pollingStation, monitoringObserver, this, arrivalTime, departureTime, answers);
    }

    public void FillIn(PollingStationInformation filledInForm,
        DateTime? arrivalTime,
        DateTime? departureTime,
        List<BaseAnswer> answers)
    {
        var validationResult = AnswersValidator.GetValidationResults(answers, Questions);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        filledInForm.UpdateDetails(arrivalTime, departureTime, answers);
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private PollingStationInformationForm()
    {

    }
#pragma warning restore CS8618
}
