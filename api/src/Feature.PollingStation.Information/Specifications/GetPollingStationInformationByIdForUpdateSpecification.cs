using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Feature.PollingStation.Information.Specifications;

public sealed class GetPollingStationInformationByIdForUpdateSpecification : SingleResultSpecification<PollingStationInformation>
{
    public GetPollingStationInformationByIdForUpdateSpecification(Guid electionRoundId, Guid id)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId && x.Id == id);
    }
}
