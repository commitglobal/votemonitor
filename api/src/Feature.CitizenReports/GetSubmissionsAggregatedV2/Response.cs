using Module.Answers.Aggregators;

namespace Feature.CitizenReports.GetSubmissionsAggregatedV2;

public class Response
{
    public CitizenReportFormSubmissionsAggregateV2 SubmissionsAggregate { get; set; } = null!;
}
