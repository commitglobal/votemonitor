using Module.Answers.Aggregators;

namespace Feature.Form.Submissions.GetAggregatedV2;

public class Response
{
    public FormSubmissionsAggregateV2 SubmissionsAggregate { get; set; } = null!;
}
