using Vote.Monitor.Answer.Module.Aggregators.Extensions;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Answer.Module.Aggregators;

public class RatingAnswerAggregate : BaseAnswerAggregate
{
    private readonly List<Response<int>> _answers = new();
    public IReadOnlyList<Response<int>> Answers => _answers.AsReadOnly();

    private readonly Dictionary<int, int> _answersHistogram;
    public IReadOnlyDictionary<int, int> AnswersHistogram => _answersHistogram.AsReadOnly();

    public int Min { get; set; } = int.MaxValue;
    public int Max { get; set; } = int.MinValue;
    public decimal Average { get; set; }

    public RatingAnswerAggregate(RatingQuestion question, int displayOrder) : base(question, displayOrder)
    {
        _answersHistogram = question.Scale.ToRange().ToDictionary(x => x, _ => 0);
    }

    protected override void QuestionSpecificAggregate(Guid responderId, BaseAnswer answer)
    {
        if (answer is not RatingAnswer ratingAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answers.Add(new Response<int>(responderId, ratingAnswer.Value));
        _answersHistogram.IncrementFor(ratingAnswer.Value);


        Min = Math.Min(ratingAnswer.Value, Min);
        Max = Math.Max(ratingAnswer.Value, Max);

        Average = Average.RecomputeAverage(ratingAnswer.Value, _answers.Count);
    }
}
