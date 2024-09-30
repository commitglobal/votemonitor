namespace Feature.IssueReports.Specifications;

public sealed class GetIssueReportSpecification : SingleResultSpecification<IssueReportAggregate>
{
    public GetIssueReportSpecification(Guid electionRoundId, Guid observerId, Guid formId, Guid issueReportId)
    {
        Query.Where(x =>
            x.ElectionRoundId == electionRoundId
            && x.FormId == formId
            && x.MonitoringObserver.ObserverId == observerId
            && x.Id == issueReportId);
    }
}