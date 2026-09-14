using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;

namespace Feature.CitizenReports.Comments.Specifications;

public sealed class GetCitizenReportInElectionRoundSpecification : Specification<CitizenReport>
{
    public GetCitizenReportInElectionRoundSpecification(Guid electionRoundId, Guid citizenReportId)
    {
        Query.Where(x => x.Id == citizenReportId && x.ElectionRoundId == electionRoundId);
    }
}
