using System.Text.Json.Serialization;
using Module.Answers.Models;
using Module.Answers.Aggregators.Extensions;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Answers.Aggregators;

public class DateAnswerAggregate : BaseAnswerAggregate
{
    public DateAnswerAggregate(DateQuestion question, int displayOrder) : base(question, displayOrder)
    {
    }

    [JsonConstructor]
    private DateAnswerAggregate()
    {
    }

    private Dictionary<string, int> _answersHistogram = new();

    [JsonInclude]
    public IReadOnlyDictionary<string, int> AnswersHistogram
    {
        get => _answersHistogram.AsReadOnly();
        private set => _answersHistogram = new Dictionary<string, int>(value);
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer, Guid? pollingStationId)
    {
        if (answer is not DateAnswer dateAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answersHistogram.IncrementFor(GetBucketName(dateAnswer.Date));
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer, Guid? pollingStationId)
    {
        if (answer is not DateAnswerModel dateAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answersHistogram.IncrementFor(GetBucketName(dateAnswer.Date));
    }

    private string GetBucketName(DateTime date)
    {
        // Truncate minutes, seconds, and milliseconds by creating a new DateTime
        var truncatedDate = new DateTime(
            date.Year,
            date.Month,
            date.Day,
            date.Hour,
            0, 0, 0,
            date.Kind
        );

        return truncatedDate.ToString("o");
    }
}
