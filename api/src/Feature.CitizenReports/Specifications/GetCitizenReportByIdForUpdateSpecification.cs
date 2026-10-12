namespace Feature.CitizenReports.Specifications;

public sealed class GetCitizenReportByIdForUpdateSpecification : SingleResultSpecification<CitizenReportAggregate>
{
    public GetCitizenReportByIdForUpdateSpecification(Guid electionRoundId, Guid citizenReportId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId && x.Id == citizenReportId);
    }
}
