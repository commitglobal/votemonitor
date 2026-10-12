using System.Text.Json.Serialization;
using Module.Answers.Models;
using Module.Answers.Aggregators.Extensions;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Answers.Aggregators;

public class RatingAnswerAggregate : BaseAnswerAggregate
{
    private int _numberOfAnswersAggregated = 0;
    private Dictionary<int, int> _answersHistogram = new();

    [JsonInclude]
    public IReadOnlyDictionary<int, int> AnswersHistogram
    {
        get => _answersHistogram.AsReadOnly();
        private set => _answersHistogram = new Dictionary<int, int>(value);
    }

    [JsonInclude] public int Min { get; private set; } = int.MaxValue;
    [JsonInclude] public int Max { get; private set; } = int.MinValue;
    [JsonInclude] public decimal Average { get; private set; }


    [JsonConstructor]
    private RatingAnswerAggregate()
    {
    }

    public RatingAnswerAggregate(RatingQuestion question, int displayOrder) : base(question, displayOrder)
    {
        _answersHistogram = question.Scale.ToRange().ToDictionary(x => x, _ => 0);
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer)
    {
        if (answer is not RatingAnswer ratingAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }
        _numberOfAnswersAggregated++;

        _answersHistogram.IncrementFor(ratingAnswer.Value);

        Min = Math.Min(ratingAnswer.Value, Min);
        Max = Math.Max(ratingAnswer.Value, Max);

        Average = Average.RecomputeAverage(ratingAnswer.Value, _numberOfAnswersAggregated);
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer)
    {
        if (answer is not RatingAnswerModel ratingAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }
        _numberOfAnswersAggregated++;

        _answersHistogram.IncrementFor(ratingAnswer.Value);

        Min = Math.Min(ratingAnswer.Value, Min);
        Max = Math.Max(ratingAnswer.Value, Max);

        Average = Average.RecomputeAverage(ratingAnswer.Value, _numberOfAnswersAggregated);
    }
}
