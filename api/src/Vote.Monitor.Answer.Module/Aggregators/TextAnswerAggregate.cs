using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Answer.Module.Aggregators;

public record TextResponse(Guid SubmissionId, Guid ResponderId, string Value);

public class TextAnswerAggregate(TextQuestion question, int displayOrder) : BaseAnswerAggregate(question, displayOrder)
{
    private readonly List<TextResponse> _answers = new();
    public IReadOnlyList<TextResponse> Answers => _answers.AsReadOnly();

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer)
    {
        if (answer is not TextAnswer textAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answers.Add(new TextResponse(submissionId, monitoringObserverId, textAnswer.Text));
    }
}
