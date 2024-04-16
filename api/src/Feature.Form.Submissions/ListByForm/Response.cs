using Vote.Monitor.Answer.Module.Aggregators;

namespace Feature.Form.Submissions.ListByForm;

public record Response
{
    public List<FormSubmissionsAggregate> FormSubmissionsAggregates { get; set; } = [];
}
