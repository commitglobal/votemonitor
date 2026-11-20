using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

public class FormSubmission : IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid PollingStationId { get; private set; }
    public PollingStation PollingStation { get; private set; }
    public Guid MonitoringObserverId { get; private set; }
    public MonitoringObserver MonitoringObserver { get; private set; }
    public Guid FormId { get; private set; }
    public Form Form { get; private set; }
    public int NumberOfQuestionsAnswered { get; private set; }

    /// <summary>The questions the observer was shown (display logic applied) and left unanswered.</summary>
    public int NumberOfUnansweredQuestions { get; private set; }
    public int NumberOfFlaggedAnswers { get; private set; }
    public SubmissionFollowUpStatus FollowUpStatus { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }
    public IReadOnlyList<BaseAnswer> Answers { get; private set; } = new List<BaseAnswer>().AsReadOnly();

    private FormSubmission(
        Guid submissionId,
        ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        Form form,
        List<BaseAnswer> answers,
        int numberOfQuestionsAnswered,
        int numberOfUnansweredQuestions,
        int numberOfFlaggedAnswers,
        bool? isCompleted,
        DateTime createdAt,
        DateTime lastUpdatedAt)
    {
        Id = submissionId;
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        PollingStation = pollingStation;
        PollingStationId = pollingStation.Id;
        MonitoringObserver = monitoringObserver;
        MonitoringObserverId = monitoringObserver.Id;
        Form = form;
        FormId = form.Id;
        Answers = answers.ToList().AsReadOnly();
        NumberOfQuestionsAnswered = numberOfQuestionsAnswered;
        NumberOfUnansweredQuestions = numberOfUnansweredQuestions;
        NumberOfFlaggedAnswers = numberOfFlaggedAnswers;
        FollowUpStatus = SubmissionFollowUpStatus.NotApplicable;
        CreatedAt = createdAt;
        LastUpdatedAt = lastUpdatedAt;

        if (isCompleted.HasValue)
        {
            IsCompleted = isCompleted.Value;
        }
    }
    
    internal static FormSubmission Create(ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        Form form,
        List<BaseAnswer> answers,
        int numberOfQuestionAnswered,
        int numberOfUnansweredQuestions,
        int numberOfFlaggedAnswers,
        bool? isCompleted,
        DateTime createdAt,
        DateTime lastUpdatedAt) =>
        new(Guid.NewGuid(),
            electionRound,
            pollingStation,
            monitoringObserver,
            form,
            answers,
            numberOfQuestionAnswered,
            numberOfUnansweredQuestions,
            numberOfFlaggedAnswers,
            isCompleted,
            createdAt,
            lastUpdatedAt);
    
    internal static FormSubmission CreateV2(
        Guid submissionId,
        ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        Form form,
        List<BaseAnswer> answers,
        int numberOfQuestionAnswered,
        int numberOfUnansweredQuestions,
        int numberOfFlaggedAnswers,
        bool? isCompleted,
        DateTime createdAt,
        DateTime lastUpdatedAt) =>
        new(
            submissionId,
            electionRound,
            pollingStation,
            monitoringObserver,
            form,
            answers,
            numberOfQuestionAnswered,
            numberOfUnansweredQuestions,
            numberOfFlaggedAnswers,
            isCompleted,
            createdAt,
            lastUpdatedAt);

    internal void Update(IEnumerable<BaseAnswer>? answers,
        int? numberOfQuestionsAnswered,
        int? numberOfUnansweredQuestions,
        int? numberOfFlaggedAnswers,
        bool? isCompleted, 
        DateTime lastUpdatedAt)
    {
        if (answers is not null)
        {
            Answers = answers.ToList().AsReadOnly();
        }

        if (numberOfFlaggedAnswers is not null)
        {
            NumberOfFlaggedAnswers = numberOfFlaggedAnswers.Value;
        }

        if (numberOfQuestionsAnswered is not null)
        {
            NumberOfQuestionsAnswered = numberOfQuestionsAnswered.Value;
        }

        if (numberOfUnansweredQuestions is not null)
        {
            NumberOfUnansweredQuestions = numberOfUnansweredQuestions.Value;
        }

        if (isCompleted.HasValue)
        {
            IsCompleted = isCompleted.Value;
        }
        
        LastUpdatedAt = lastUpdatedAt;
    }

    public void ClearAnswers()
    {
        Answers = [];
        NumberOfFlaggedAnswers = 0;
        NumberOfQuestionsAnswered = 0;
        NumberOfUnansweredQuestions = Form is null ? 0 : AnswersHelpers.CountAnswers(Form.Questions, []).Unanswered;
    }

    public void UpdateFollowUpStatus(SubmissionFollowUpStatus followUpStatus)
    {
        FollowUpStatus = followUpStatus;
    }

    public void MoveToPollingStation(PollingStation pollingStation)
    {
        PollingStation = pollingStation;
        PollingStationId = pollingStation.Id;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private FormSubmission()
    {
    }
#pragma warning restore CS8618
}
