using System.Text.Json.Serialization;
using Module.Answers.Models;
using Module.Answers.Aggregators.Extensions;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Answers.Aggregators;

public class SingleSelectAnswerAggregate : BaseAnswerAggregate
{
    private List<TextResponse> _freeTexts = [];
    private Dictionary<Guid, int> _answersHistogram = new();

    // the polling stations seen while aggregating in memory; the histogram computed by the database is kept as is
    private DistinctPollingStations<Guid>? _pollingStations;
    private IReadOnlyDictionary<Guid, int>? _pollingStationsHistogram;
    [JsonInclude]
    public IReadOnlyDictionary<Guid, int> AnswersHistogram
    {
        get => _answersHistogram.AsReadOnly();
        private set => _answersHistogram = new Dictionary<Guid, int>(value);
    }

    /// <summary>
    /// Number of distinct submissions that picked each option. Only set when computed by the database.
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<Guid, int>? SubmissionsHistogram { get; private set; }

    /// <summary>
    /// Number of distinct polling stations where each option was picked. Not set for citizen reports.
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<Guid, int>? PollingStationsHistogram
    {
        get => _pollingStations?.ToHistogram() ?? _pollingStationsHistogram;
        private set => _pollingStationsHistogram = value;
    }

    /// <summary>
    /// Number of distinct citizen report locations where each option was picked. Only set when computed by the database,
    /// and only for citizen reports.
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<Guid, int>? LocationsHistogram { get; private set; }
    /// <summary>
    /// Not set when the free texts are referenced by id (<see cref="FreeTextIds"/>).
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<TextResponse>? FreeTexts
    {
        get => FreeTextIds is null ? _freeTexts.ToList().AsReadOnly() : null;
        private set => _freeTexts = value?.ToList() ?? [];
    }

    /// <summary>
    /// Ids of the texts in the aggregate's Texts. Only set when computed by the database.
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<int>? FreeTextIds { get; private set; }

    [JsonConstructor]
    private SingleSelectAnswerAggregate()
    {
    }

    public SingleSelectAnswerAggregate(SingleSelectQuestion question, int displayOrder) : base(question, displayOrder)
    {
        _answersHistogram = question.Options.ToDictionary(o => o.Id, _ => 0);
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer, Guid? pollingStationId)
    {
        if (answer is not SingleSelectAnswer singleSelectAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answersHistogram.IncrementFor(singleSelectAnswer.Selection.OptionId);
        AddPollingStation(singleSelectAnswer.Selection.OptionId, pollingStationId);
        
        var selection = singleSelectAnswer.Selection;
        var questionOption = ((SingleSelectQuestion)Question).Options.FirstOrDefault(o => o.Id == selection.OptionId);
        if (questionOption != null && questionOption.IsFreeText && !string.IsNullOrWhiteSpace(selection.Text))
        {
            _freeTexts.Add(new TextResponse(submissionId, monitoringObserverId, selection.Text));
        }
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer, Guid? pollingStationId)
    {
        if (answer is not SingleSelectAnswerModel singleSelectAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answersHistogram.IncrementFor(singleSelectAnswer.Selection.OptionId);
        AddPollingStation(singleSelectAnswer.Selection.OptionId, pollingStationId);
        
        var selection = singleSelectAnswer.Selection;
        var questionOption = ((SingleSelectQuestion)Question).Options.FirstOrDefault(o => o.Id == selection.OptionId);
        if (questionOption != null && questionOption.IsFreeText && !string.IsNullOrWhiteSpace(selection.Text))
        {
            _freeTexts.Add(new TextResponse(submissionId, monitoringObserverId, selection.Text));
        }
    }

    private void AddPollingStation(Guid key, Guid? pollingStationId)
    {
        if (pollingStationId is null)
        {
            return;
        }

        _pollingStations ??= new DistinctPollingStations<Guid>(((SingleSelectQuestion)Question).Options.Select(o => o.Id));
        _pollingStations.Add(key, pollingStationId.Value);
    }
}
