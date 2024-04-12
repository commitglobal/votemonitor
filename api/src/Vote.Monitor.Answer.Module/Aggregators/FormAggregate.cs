using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Vote.Monitor.Answer.Module.Aggregators;

public class FormAggregate
{
    public Guid ElectionRoundId { get; }
    public Guid MonitoringNgoId { get; }
    public Guid FormId { get; }

    private readonly HashSet<Guid> _responders = new();
    public IReadOnlyList<Guid> Responders => _responders.ToList().AsReadOnly();

    private readonly Dictionary<Guid, HashSet<Guid>> _pollingStations = new();

    /// <summary>
    /// Filled in forms grouped by polling station id
    /// </summary>
    public IReadOnlyDictionary<Guid, List<Guid>> PollingStations => _pollingStations
        .ToDictionary(x => x.Key, v => v.Value.ToList())
        .AsReadOnly();

    public int SubmissionCount { get; private set; }

    public IReadOnlyDictionary<Guid, BaseAnswerAggregate> Aggregates { get; }

    public FormAggregate(Form form)
    {
        ElectionRoundId = form.ElectionRoundId;
        MonitoringNgoId = form.MonitoringNgoId;
        FormId = form.Id;

        Aggregates = form
            .Questions
            .Select(AnswerAggregateFactory.Map)
            .ToDictionary(a => a.QuestionId, x => x)
            .AsReadOnly();
    }

    public FormAggregate AggregateAnswers(FormSubmission formSubmission)
    {
        var responderId = formSubmission.MonitoringObserverId;

        _responders.Add(responderId);

        _pollingStations.AddOrUpdate(formSubmission.PollingStationId,
            createFunc: () => [formSubmission.Id],
            updateFunc: set => set.Add(formSubmission.Id));

        SubmissionCount++;

        foreach (var answer in formSubmission.Answers)
        {
            Aggregates[answer.QuestionId].Aggregate(responderId, answer);
        }

        return this;
    }
}
