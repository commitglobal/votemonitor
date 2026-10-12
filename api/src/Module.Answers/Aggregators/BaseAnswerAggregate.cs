using System.Text.Json.Serialization;
using Module.Answers.Models;
using PolyJson;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Answers.Aggregators;


[PolyJsonConverter(distriminatorPropertyName: "$questionType")]

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

    [JsonInclude] public Guid QuestionId { get; private set; }
    public int DisplayOrder { get; set; }
    [JsonInclude] public BaseQuestion Question { get; private set; } = null!;
    [JsonInclude] public int AnswersAggregated { get; private set; }

    /// <summary>
    /// Used when deserializing an aggregate computed by the database.
    /// </summary>
    protected BaseAnswerAggregate()
    {
    }

    internal BaseAnswerAggregate(BaseQuestion question, int displayOrder)
    {
        QuestionId = question.Id;
        Question = question;
        DisplayOrder = displayOrder;
    }

    public void Aggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer)
    {
        AnswersAggregated += 1;
        QuestionSpecificAggregate(submissionId, monitoringObserverId, answer);
    }


    protected abstract void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer);
    
    public void Aggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer)
    {
        AnswersAggregated += 1;
        QuestionSpecificAggregate(submissionId, monitoringObserverId, answer);
    }


    protected abstract void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer);
}
