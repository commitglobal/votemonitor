using System.Text.Json.Serialization;
using Module.Answers.Models;
using Module.Answers.Aggregators.Extensions;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Answers.Aggregators;

public class MultiSelectAnswerAggregate : BaseAnswerAggregate
{
    private List<TextResponse> _freeTexts = [];

    private Dictionary<Guid, int> _answersHistogram = new();
    private readonly bool _hasFreeTextOption;
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
    /// Number of distinct polling stations where each option was picked. Only set when computed by the database,
    /// and not for citizen reports.
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<Guid, int>? PollingStationsHistogram { get; private set; }

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
    public IReadOnlyCollection<TextResponse>? FreeTexts
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
    private MultiSelectAnswerAggregate()
    {
    }

    public MultiSelectAnswerAggregate(MultiSelectQuestion question, int displayOrder) : base(question, displayOrder)
    {
        _answersHistogram = question.Options.ToDictionary(o => o.Id, _ => 0);
        _hasFreeTextOption = question.Options.Any(o => o.IsFreeText);
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer)
    {
        if (answer is not MultiSelectAnswer multiSelectAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        foreach (var selectedOption in multiSelectAnswer.Selection)
        {
            _answersHistogram.IncrementFor(selectedOption.OptionId);
        }

        var questionOptions = ((MultiSelectQuestion)Question).Options;
        foreach (var selectedOption in multiSelectAnswer.Selection)
        {
            var questionOption = questionOptions.FirstOrDefault(o => o.Id == selectedOption.OptionId);
            if (questionOption != null && questionOption.IsFreeText && !string.IsNullOrWhiteSpace(selectedOption.Text))
            {
                _freeTexts.Add(new TextResponse(submissionId, monitoringObserverId, selectedOption.Text));
            }
        }
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer)
    {
        if (answer is not MultiSelectAnswerModel multiSelectAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        foreach (var selectedOption in multiSelectAnswer.Selection)
        {
            _answersHistogram.IncrementFor(selectedOption.OptionId);
        }

        var questionOptions = ((MultiSelectQuestion)Question).Options;
        foreach (var selectedOption in multiSelectAnswer.Selection)
        {
            var questionOption = questionOptions.FirstOrDefault(o => o.Id == selectedOption.OptionId);
            if (questionOption != null && questionOption.IsFreeText && !string.IsNullOrWhiteSpace(selectedOption.Text))
            {
                _freeTexts.Add(new TextResponse(submissionId, monitoringObserverId, selectedOption.Text));
            }
        }
    }
}
