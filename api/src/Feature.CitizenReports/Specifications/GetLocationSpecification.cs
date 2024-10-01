using Vote.Monitor.Domain.Entities.LocationAggregate;

namespace Feature.CitizenReports.Specifications;

public sealed class GetLocationSpecification : SingleResultSpecification<Location>
{
    public GetLocationSpecification(Guid electionRoundId, Guid locationId)
    {
        Query.Where(x => x.Id == locationId && x.ElectionRoundId == electionRoundId);
    }
}