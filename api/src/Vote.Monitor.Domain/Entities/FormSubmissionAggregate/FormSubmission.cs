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
    public int NumberOfQuestionAnswered { get; private set; }
    public int NumberOfFlaggedAnswers { get; private set; }
    public IReadOnlyList<BaseAnswer> Answers { get; private set; } = new List<BaseAnswer>().AsReadOnly();

    private FormSubmission(
        ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        Form form,
        List<BaseAnswer> answers,
        int numberOfQuestionAnswered,
        int numberOfFlaggedAnswers) : base(Guid.NewGuid())
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
        NumberOfQuestionAnswered = numberOfQuestionAnswered;
        NumberOfFlaggedAnswers = numberOfFlaggedAnswers;
    }

    internal static FormSubmission Create(
        ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        Form form,
        List<BaseAnswer> answers,
        int numberOfQuestionAnswered,
        int numberOfFlaggedAnswers) =>
        new(electionRound, pollingStation, monitoringObserver, form, answers, numberOfQuestionAnswered, numberOfFlaggedAnswers);

    internal void UpdateAnswers(int numberOfQuestionAnswered, int numberOfFlaggedAnswers, IEnumerable<BaseAnswer> answers)
    {
        NumberOfFlaggedAnswers = numberOfFlaggedAnswers;
        NumberOfQuestionAnswered = numberOfQuestionAnswered;
        Answers = answers.ToList().AsReadOnly();
    }

    public void ClearAnswers()
    {
        Answers = [];
        NumberOfFlaggedAnswers = 0;
        NumberOfQuestionAnswered = 0;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    private FormSubmission()
    {

    }
#pragma warning restore CS8618
}
