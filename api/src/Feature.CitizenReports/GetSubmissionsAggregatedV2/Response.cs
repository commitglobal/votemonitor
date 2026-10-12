using Module.Answers.Aggregators;

namespace Feature.CitizenReports.GetSubmissionsAggregatedV2;

public class Response
{
    public CitizenReportFormSubmissionsAggregate SubmissionsAggregate { get; set; } = null!;
}
