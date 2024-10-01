namespace Feature.IncidentReports.Specifications;

public sealed class GetIncidentReportByIdSpecification : SingleResultSpecification<IncidentReportAggregate>
{
    public GetIncidentReportByIdSpecification(Guid electionId, Guid formId, Guid reportId)
    {
        Query.Where(x => x.ElectionRoundId == electionId && x.Form.Id == formId && x.Id == reportId);
    }
}