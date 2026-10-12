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

    [JsonInclude]
    public IReadOnlyList<TextResponse> Answers
    {
        get => _answers.AsReadOnly();
        private set => _answers = value.ToList();
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer)
    {
        if (answer is not TextAnswer textAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answers.Add(new TextResponse(submissionId, monitoringObserverId, textAnswer.Text));
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer)
    {
        if (answer is not TextAnswerModel textAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answers.Add(new TextResponse(submissionId, monitoringObserverId, textAnswer.Text));
    }
}
