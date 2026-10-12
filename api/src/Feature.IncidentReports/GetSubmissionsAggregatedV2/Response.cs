using Module.Answers.Aggregators;

namespace Feature.IncidentReports.GetSubmissionsAggregatedV2;

public class Response
{
    public FormSubmissionsAggregate SubmissionsAggregate { get; set; } = null!;
}
