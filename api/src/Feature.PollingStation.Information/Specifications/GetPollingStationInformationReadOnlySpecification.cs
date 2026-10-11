using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Feature.PollingStation.Information.Specifications;

public sealed class GetPollingStationInformationReadOnlySpecification : SingleResultSpecification<PollingStationInformation>
{
    public GetPollingStationInformationReadOnlySpecification(Guid electionRoundId, Guid pollingStationId, Guid observerId)
    {
        Query.AsNoTracking();

        Query.Where(x =>
            x.ElectionRoundId == electionRoundId
            && x.PollingStationId == pollingStationId
            && x.MonitoringObserver.ObserverId == observerId
            && x.MonitoringObserver.ElectionRoundId == electionRoundId);
    }
}
