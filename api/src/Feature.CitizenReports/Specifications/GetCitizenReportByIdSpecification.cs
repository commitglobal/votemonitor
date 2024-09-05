namespace Feature.CitizenReports.Specifications;

public sealed class GetCitizenReportByIdSpecification : SingleResultSpecification<CitizenReportAggregate>
{
    public GetCitizenReportByIdSpecification(Guid electionId, Guid formId, Guid reportId)
    {
        Query.Where(x => x.ElectionRoundId == electionId && x.Form.Id == formId && x.Id == reportId);
    }
}