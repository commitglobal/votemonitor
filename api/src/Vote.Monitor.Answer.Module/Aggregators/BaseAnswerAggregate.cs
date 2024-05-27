using System;
using System.Text.Json.Serialization;
using PolyJson;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Answer.Module.Aggregators;


[PolyJsonConverter(distriminatorPropertyName: "$aggregateType")]

[PolyJsonConverter.SubType(typeof(TextAnswerAggregate), AnswerAggregateTypes.TextAnswerAggregate)]
[PolyJsonConverter.SubType(typeof(NumberAnswerAggregate), AnswerAggregateTypes.NumberAnswerAggregate)]
[PolyJsonConverter.SubType(typeof(DateAnswerAggregate), AnswerAggregateTypes.DateAnswerAggregate)]
[PolyJsonConverter.SubType(typeof(SingleSelectAnswerAggregate), AnswerAggregateTypes.SingleSelectAnswerAggregate)]
[PolyJsonConverter.SubType(typeof(MultiSelectAnswerAggregate), AnswerAggregateTypes.MultiSelectAnswerAggregate)]
[PolyJsonConverter.SubType(typeof(RatingAnswerAggregate), AnswerAggregateTypes.RatingAnswerAggregate)]
public abstract class BaseAnswerAggregate
{
    [JsonPropertyName("$questionType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Discriminator => DiscriminatorValue.Get(GetType());

    public Guid QuestionId { get; }
    public int DisplayOrder { get; set; }
    public BaseQuestion Question { get; }
    public int AnswersAggregated { get; private set; }

    private readonly HashSet<Guid> _responders = new();
    public IReadOnlyList<Guid> Responders => _responders.ToList().AsReadOnly();

    internal BaseAnswerAggregate(BaseQuestion question, int displayOrder)
    {
        QuestionId = question.Id;
        Question = question;
        DisplayOrder = displayOrder;
    }

    public void Aggregate(Guid responderId, BaseAnswer answer)
    {
        _responders.Add(responderId);

        AnswersAggregated += 1;
        QuestionSpecificAggregate(responderId, answer);
    }

    protected abstract void QuestionSpecificAggregate(Guid responder, BaseAnswer answer);
}
