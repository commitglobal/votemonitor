using Vote.Monitor.Answer.Module.Aggregators.Extensions;
using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Answer.Module.Aggregators;

public class DateAnswerAggregate(DateQuestion question, int displayOrder) : BaseAnswerAggregate(question, displayOrder)
{
    private readonly Dictionary<string, int> _answersHistogram = new();
    public IReadOnlyDictionary<string, int> AnswersHistogram => _answersHistogram.AsReadOnly();

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer)
    {
        if (answer is not DateAnswer dateAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answersHistogram.IncrementFor(GetBucketName(dateAnswer.Date));
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer)
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
