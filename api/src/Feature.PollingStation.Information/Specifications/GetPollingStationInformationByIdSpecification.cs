using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Feature.PollingStation.Information.Specifications;

public sealed class GetPollingStationInformationByIdSpecification : SingleResultSpecification<PollingStationInformation>
{
    public GetPollingStationInformationByIdSpecification(Guid electionRoundId, Guid observerId, Guid id)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId
                         && x.MonitoringObserver.ObserverId == observerId
                         && x.Id == id);
    }
}
