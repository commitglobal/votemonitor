namespace Feature.IssueReports.Specifications;

public sealed class GetIssueReportByIdSpecification : SingleResultSpecification<IssueReportAggregate>
{
    public GetIssueReportByIdSpecification(Guid electionId, Guid formId, Guid reportId)
    {
        Query.Where(x => x.ElectionRoundId == electionId && x.Form.Id == formId && x.Id == reportId);
    }
}