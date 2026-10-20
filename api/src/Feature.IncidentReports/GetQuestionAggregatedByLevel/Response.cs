using Module.Answers.Aggregators;

namespace Feature.IncidentReports.GetQuestionAggregatedByLevel;

public class Response
{
    public QuestionAggregateByLevel QuestionAggregate { get; set; } = null!;
}
