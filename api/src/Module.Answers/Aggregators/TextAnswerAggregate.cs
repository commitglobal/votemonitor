using System.Text.Json.Serialization;
using Module.Answers.Models;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Answers.Aggregators;

public record TextResponse(Guid SubmissionId, Guid ResponderId, string Value);

public class TextAnswerAggregate : BaseAnswerAggregate
{
    public TextAnswerAggregate(TextQuestion question, int displayOrder) : base(question, displayOrder)
    {
    }

    [JsonConstructor]
    private TextAnswerAggregate()
    {
    }

    private List<TextResponse> _answers = new();

    /// <summary>
    /// Not set when the texts are referenced by id (<see cref="AnswerIds"/>).
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<TextResponse>? Answers
    {
        get => AnswerIds is null ? _answers.AsReadOnly() : null;
        private set => _answers = value?.ToList() ?? [];
    }

    /// <summary>
    /// Ids of the texts in the aggregate's Texts. Only set when computed by the database.
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<int>? AnswerIds { get; private set; }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer, Guid? pollingStationId)
    {
        if (answer is not TextAnswer textAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answers.Add(new TextResponse(submissionId, monitoringObserverId, textAnswer.Text));
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer, Guid? pollingStationId)
    {
        if (answer is not TextAnswerModel textAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answers.Add(new TextResponse(submissionId, monitoringObserverId, textAnswer.Text));
    }
}
