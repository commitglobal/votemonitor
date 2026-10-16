using System.Globalization;
using System.Text.Json.Serialization;
using Module.Answers.Models;
using Module.Answers.Aggregators.Extensions;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Answers.Aggregators;

public class TimeAnswerAggregate : BaseAnswerAggregate
{
    public TimeAnswerAggregate(TimeQuestion question, int displayOrder) : base(question, displayOrder)
    {
    }

    [JsonConstructor]
    private TimeAnswerAggregate()
    {
    }

    private Dictionary<string, int> _answersHistogram = new();

    [JsonInclude]
    public IReadOnlyDictionary<string, int> AnswersHistogram
    {
        get => _answersHistogram.AsReadOnly();
        private set => _answersHistogram = new Dictionary<string, int>(value);
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer)
    {
        if (answer is not TimeAnswer timeAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answersHistogram.IncrementFor(GetBucketName(timeAnswer.Time));
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer)
    {
        if (answer is not TimeAnswerModel timeAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answersHistogram.IncrementFor(GetBucketName(timeAnswer.Time));
    }

    private static string GetBucketName(TimeOnly time) => time.ToString("HH:mm", CultureInfo.InvariantCulture);
}
