using Vote.Monitor.Answer.Module.Aggregators.Extensions;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Answer.Module.Aggregators;

public class NumberAnswerAggregate(NumberQuestion question, int displayOrder) : BaseAnswerAggregate(question, displayOrder)
{
    private int _numberOfAnswersAggregated;
    public int Min { get; private set; } = int.MaxValue;
    public int Max { get; private set; } = int.MinValue;
    public decimal Average { get; private set; }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer)
    {
        if (answer is not NumberAnswer numberAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _numberOfAnswersAggregated++;

        Min = Math.Min(numberAnswer.Value, Min);
        Max = Math.Max(numberAnswer.Value, Max);

        Average = Average.RecomputeAverage(numberAnswer.Value, _numberOfAnswersAggregated);
    }
}
