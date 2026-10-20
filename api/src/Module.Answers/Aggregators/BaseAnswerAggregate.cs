using System.Text.Json.Serialization;
using Module.Answers.Models;
using PolyJson;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Answers.Aggregators;


[PolyJsonConverter(distriminatorPropertyName: "$questionType")]

[PolyJsonConverter.SubType(typeof(TextAnswerAggregate), AnswerAggregateTypes.TextAnswerAggregate)]
[PolyJsonConverter.SubType(typeof(NumberAnswerAggregate), AnswerAggregateTypes.NumberAnswerAggregate)]
[PolyJsonConverter.SubType(typeof(DateAnswerAggregate), AnswerAggregateTypes.DateAnswerAggregate)]
[PolyJsonConverter.SubType(typeof(TimeAnswerAggregate), AnswerAggregateTypes.TimeAnswerAggregate)]
[PolyJsonConverter.SubType(typeof(DateTimeAnswerAggregate), AnswerAggregateTypes.DateTimeAnswerAggregate)]
[PolyJsonConverter.SubType(typeof(SingleSelectAnswerAggregate), AnswerAggregateTypes.SingleSelectAnswerAggregate)]
[PolyJsonConverter.SubType(typeof(MultiSelectAnswerAggregate), AnswerAggregateTypes.MultiSelectAnswerAggregate)]
[PolyJsonConverter.SubType(typeof(RatingAnswerAggregate), AnswerAggregateTypes.RatingAnswerAggregate)]
public abstract class BaseAnswerAggregate
{
    [JsonPropertyName("$questionType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Discriminator => DiscriminatorValue.Get(GetType());

    [JsonInclude] public Guid QuestionId { get; private set; }
    public int DisplayOrder { get; set; }
    /// <summary>
    /// Not set on the aggregates of a location breakdown (see <see cref="LocationAggregates"/>).
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BaseQuestion Question { get; private set; } = null!;
    [JsonInclude] public int AnswersAggregated { get; private set; }

    // the polling stations / citizen report locations seen while aggregating in memory; the counts computed by the database are kept as is
    private HashSet<Guid>? _pollingStations;
    private HashSet<Guid>? _locations;
    private int? _pollingStationsCount;
    private int? _locationsCount;

    /// <summary>
    /// Number of distinct polling stations of the answers aggregated. Not set for citizen reports.
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PollingStationsCount
    {
        get => _pollingStations?.Count ?? _pollingStationsCount;
        private set => _pollingStationsCount = value;
    }

    /// <summary>
    /// Number of distinct citizen report locations of the answers aggregated. Only set for citizen reports.
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? LocationsCount
    {
        get => _locations?.Count ?? _locationsCount;
        private set => _locationsCount = value;
    }

    /// <summary>
    /// Ids of the question's notes in the aggregate's Notes. Only set when computed by the database.
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<int>? NoteIds { get; private set; }

    /// <summary>
    /// Ids of the question's attachments in the aggregate's Attachments. Only set when computed by the database.
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<int>? AttachmentIds { get; private set; }

    /// <summary>
    /// The question's aggregate per location id (see <see cref="AggregateLocation"/>), without the question itself and
    /// referencing only the texts, notes and attachments of the location's submissions. Only set when computed by the database.
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<int, BaseAnswerAggregate>? LocationAggregates { get; private set; }

    /// <summary>
    /// Used when deserializing an aggregate computed by the database.
    /// </summary>
    protected BaseAnswerAggregate()
    {
    }

    internal BaseAnswerAggregate(BaseQuestion question, int displayOrder)
    {
        QuestionId = question.Id;
        Question = question;
        DisplayOrder = displayOrder;
    }

    /// <param name="pollingStationId">The polling station of the submission; not set for citizen reports.</param>
    /// <param name="locationId">The location of the citizen report; only set for citizen reports.</param>
    public void Aggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer, Guid? pollingStationId = null, Guid? locationId = null)
    {
        AnswersAggregated += 1;
        AddPlaces(pollingStationId, locationId);
        QuestionSpecificAggregate(submissionId, monitoringObserverId, answer, pollingStationId);
    }


    protected abstract void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer, Guid? pollingStationId);
    
    /// <param name="pollingStationId">The polling station of the submission; not set for citizen reports.</param>
    /// <param name="locationId">The location of the citizen report; only set for citizen reports.</param>
    public void Aggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer, Guid? pollingStationId = null, Guid? locationId = null)
    {
        AnswersAggregated += 1;
        AddPlaces(pollingStationId, locationId);
        QuestionSpecificAggregate(submissionId, monitoringObserverId, answer, pollingStationId);
    }

    private void AddPlaces(Guid? pollingStationId, Guid? locationId)
    {
        if (pollingStationId is not null)
        {
            _pollingStations ??= [];
            _pollingStations.Add(pollingStationId.Value);
        }

        if (locationId is not null)
        {
            _locations ??= [];
            _locations.Add(locationId.Value);
        }
    }


    protected abstract void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer, Guid? pollingStationId);
}
