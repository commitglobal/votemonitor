using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Answer.Module.Aggregators;

public abstract class BaseAnswerAggregate
{
    public Guid QuestionId { get; }
    public int AnswersAggregated { get; private set; }

    private readonly HashSet<Guid> _responders = new();
    public IReadOnlyList<Guid> Responders => _responders.ToList().AsReadOnly();

    internal BaseAnswerAggregate(Guid questionId)
    {
        QuestionId = questionId;
    }

    public void Aggregate(Guid responderId, BaseAnswer answer)
    {
        _responders.Add(responderId);

        AnswersAggregated += 1;
        QuestionSpecificAggregate(responderId, answer);
    }

    protected abstract void QuestionSpecificAggregate(Guid responder, BaseAnswer answer);
}
