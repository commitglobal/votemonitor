using System.Text.Json.Serialization;
using Module.Answers.Models;
using Module.Answers.Aggregators.Extensions;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Answers.Aggregators;

public class DateTimeAnswerAggregate : BaseAnswerAggregate
{
    public DateTimeAnswerAggregate(DateTimeQuestion question, int displayOrder) : base(question, displayOrder)
    {
    }

    [JsonConstructor]
    private DateTimeAnswerAggregate()
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
        if (answer is not DateTimeAnswer dateTimeAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answersHistogram.IncrementFor(GetBucketName(dateTimeAnswer.DateTime));
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer, Guid? pollingStationId)
    {
        if (answer is not DateTimeAnswerModel dateTimeAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answersHistogram.IncrementFor(GetBucketName(dateTimeAnswer.DateTime));
    }

    private string GetBucketName(DateTime dateTime)
    {
        // Truncate minutes, seconds, and milliseconds by creating a new DateTime
        var truncatedDate = new DateTime(
            dateTime.Year,
            dateTime.Month,
            dateTime.Day,
            dateTime.Hour,
            0, 0, 0,
            dateTime.Kind
        );

        return truncatedDate.ToString("o");
    }
}
