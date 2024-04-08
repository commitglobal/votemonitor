using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

public class FormSubmission : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid PollingStationId { get; private set; }
    public PollingStation PollingStation { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }
    public Guid FormId { get; private set; }
    public Form Form { get; private set; }

    public IReadOnlyList<BaseAnswer> Answers { get; private set; } = new List<BaseAnswer>().AsReadOnly();

    private FormSubmission(
        ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        Form form,
        List<BaseAnswer> answers) : base(Guid.NewGuid())
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        PollingStation = pollingStation;
        PollingStationId = pollingStation.Id;
        MonitoringObserver = monitoringObserver;
        MonitoringObserverId = monitoringObserver.Id;
        Form = form;
        FormId = form.Id;
        Answers = answers.ToList().AsReadOnly();
    }

    internal static FormSubmission Create(
        ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        Form form,
        List<BaseAnswer> answers) =>
        new(electionRound, pollingStation, monitoringObserver, form, answers);

    internal void UpdateAnswers(IEnumerable<BaseAnswer> answers)
    {
        Answers = answers.ToList().AsReadOnly();
    }
    
#pragma warning disable CS8618 // Required by Entity Framework
    private FormSubmission()
    {

    }
#pragma warning restore CS8618
}