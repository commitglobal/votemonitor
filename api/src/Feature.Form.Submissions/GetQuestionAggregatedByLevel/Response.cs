using Module.Answers.Aggregators;

namespace Feature.Form.Submissions.GetQuestionAggregatedByLevel;

public class Response
{
    public QuestionAggregateByLevel QuestionAggregate { get; set; } = null!;
}
