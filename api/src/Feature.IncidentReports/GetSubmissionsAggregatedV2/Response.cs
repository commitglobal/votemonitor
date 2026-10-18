using Module.Answers.Aggregators;

namespace Feature.IncidentReports.GetSubmissionsAggregatedV2;

public class Response
{
    public FormSubmissionsAggregateV2 SubmissionsAggregate { get; set; } = null!;
}
