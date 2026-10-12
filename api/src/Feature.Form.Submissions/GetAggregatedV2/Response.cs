using Module.Answers.Aggregators;

namespace Feature.Form.Submissions.GetAggregatedV2;

public class Response
{
    public FormSubmissionsAggregate SubmissionsAggregate { get; set; } = null!;
}
