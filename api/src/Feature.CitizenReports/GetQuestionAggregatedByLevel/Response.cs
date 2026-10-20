using Module.Answers.Aggregators;

namespace Feature.CitizenReports.GetQuestionAggregatedByLevel;

public class Response
{
    public QuestionAggregateByLevel QuestionAggregate { get; set; } = null!;
}
