﻿using Vote.Monitor.Domain.Entities.FormAggregate;
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
    public int NumberOfFlaggedAnswers { get; private set; }
    public SubmissionFollowUpStatus FollowUpStatus { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }
    public IReadOnlyList<BaseAnswer> Answers { get; private set; } = new List<BaseAnswer>().AsReadOnly();

    private FormSubmission(
        ElectionRound electionRound,
        PollingStation pollingStation,
        MonitoringObserver monitoringObserver,
        Form form,
        List<BaseAnswer> answers,
        int numberOfQuestionsAnswered,
        int numberOfFlaggedAnswers,
        bool? isCompleted,
        DateTime lastUpdatedAt)
    {
        Id = Guid.NewGuid();
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
        NumberOfFlaggedAnswers = numberOfFlaggedAnswers;
        FollowUpStatus = SubmissionFollowUpStatus.NotApplicable;
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
        int numberOfFlaggedAnswers,
        bool? isCompleted,
        DateTime lastUpdatedAt) =>
        new(electionRound,
            pollingStation,
            monitoringObserver,
            form,
            answers,
            numberOfQuestionAnswered,
            numberOfFlaggedAnswers,
            isCompleted,
            lastUpdatedAt);

    internal void Update(IEnumerable<BaseAnswer>? answers,
        int? numberOfQuestionsAnswered,
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
    }

    public void UpdateFollowUpStatus(SubmissionFollowUpStatus followUpStatus)
    {
        FollowUpStatus = followUpStatus;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private FormSubmission()
    {
    }
#pragma warning restore CS8618
}
