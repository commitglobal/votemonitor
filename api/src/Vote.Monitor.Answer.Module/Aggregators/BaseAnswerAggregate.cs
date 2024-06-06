using System.Text.Json.Serialization;
using PolyJson;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

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

    internal BaseAnswerAggregate(BaseQuestion question, int displayOrder)
    {
        QuestionId = question.Id;
        Question = question;
        DisplayOrder = displayOrder;
    }

    public void Aggregate(FormSubmission formSubmission, BaseAnswer answer)
    {
        AnswersAggregated += 1;
        QuestionSpecificAggregate(formSubmission, answer);
    }

    protected abstract void QuestionSpecificAggregate(FormSubmission submission, BaseAnswer answer);
}
