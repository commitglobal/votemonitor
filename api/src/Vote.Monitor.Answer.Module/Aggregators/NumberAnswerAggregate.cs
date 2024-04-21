using Vote.Monitor.Answer.Module.Aggregators.Extensions;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Answer.Module.Aggregators;

public class NumberAnswerAggregate(NumberQuestion question) : BaseAnswerAggregate(question)
{
    private readonly List<Response<int>> _answers = new();
    public IReadOnlyList<Response<int>> Answers => _answers.AsReadOnly();

    public int Min { get; private set; } = int.MaxValue;
    public int Max { get; private set; } = int.MinValue;
    public decimal Average { get; private set; }

    protected override void QuestionSpecificAggregate(Guid responderId, BaseAnswer answer)
    {
        if (answer is not NumberAnswer numberAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answers.Add(new Response<int>(responderId, numberAnswer.Value));

        Min = Math.Min(numberAnswer.Value, Min);
        Max = Math.Max(numberAnswer.Value, Max);

        Average = Average.RecomputeAverage(numberAnswer.Value, _answers.Count);
    }
}
