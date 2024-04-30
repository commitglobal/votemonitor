using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Vote.Monitor.Answer.Module.Aggregators;


public record Responder(Guid ResponderId, string FirstName, string LastName, string Email, string PhoneNumber);
public class FormSubmissionsAggregate
{
    public Guid ElectionRoundId { get; }
    public Guid MonitoringNgoId { get; }
    public Guid FormId { get; }
    public string FormCode { get; }
    public FormType FormType { get; }
    public TranslatedString Name { get; }
    public TranslatedString Description { get; }
    public string DefaultLanguage { get; }
    public string[] Languages { get; } = [];

    private readonly HashSet<Responder> _responders = new();
    public IReadOnlyList<Responder> Responders => _responders.ToList().AsReadOnly();

    private readonly Dictionary<Guid, HashSet<Guid>> _pollingStations = new();

    /// <summary>
    /// Filled in forms grouped by polling station id
    /// </summary>
    public IReadOnlyDictionary<Guid, List<Guid>> PollingStations => _pollingStations
        .ToDictionary(x => x.Key, v => v.Value.ToList())
        .AsReadOnly();

    public int SubmissionCount { get; private set; }
    public int TotalNumberOfQuestionsAnswered { get; private set; }
    public int TotalNumberOfFlaggedAnswers { get; private set; }
    /// <summary>
    /// Aggregated answers per question id
    /// </summary>
    public IReadOnlyDictionary<Guid, BaseAnswerAggregate> Aggregates { get; }

    public FormSubmissionsAggregate(Form form)
    {
        ElectionRoundId = form.ElectionRoundId;
        MonitoringNgoId = form.MonitoringNgoId;
        FormId = form.Id;
        FormCode = form.Code;
        FormType = form.FormType;
        Name = form.Name;
        Description = form.Description;
        Languages = form.Languages;
        DefaultLanguage = form.DefaultLanguage;

        Aggregates = form
            .Questions
            .Select(AnswerAggregateFactory.Map)
            .ToDictionary(a => a.QuestionId, x => x)
            .AsReadOnly();
    }

    public FormSubmissionsAggregate AggregateAnswers(FormSubmission formSubmission)
    {
        var responderId = formSubmission.MonitoringObserverId;

        var observer= formSubmission.MonitoringObserver.Observer.ApplicationUser;
        _responders.Add(new Responder(responderId, observer.FirstName, observer.LastName, observer.Email, observer.PhoneNumber));

        _pollingStations.AddOrUpdate(formSubmission.PollingStationId,
            createFunc: () => [formSubmission.Id],
            updateFunc: set => set.Add(formSubmission.Id));

        SubmissionCount++;
        TotalNumberOfFlaggedAnswers += formSubmission.NumberOfFlaggedAnswers;
        TotalNumberOfQuestionsAnswered += formSubmission.NumberOfQuestionsAnswered;

        foreach (var answer in formSubmission.Answers)
        {
            Aggregates[answer.QuestionId].Aggregate(responderId, answer);
        }

        return this;
    }
}
