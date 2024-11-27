using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Feature.PollingStation.Information.Specifications;

public sealed class
    GetPollingStationInformationForObserverSpecification : Specification<PollingStationInformation,
    PollingStationInformationModel>
{
    public GetPollingStationInformationForObserverSpecification(Guid electionRoundId, Guid observerId,
        List<Guid>? pollingStationIds)
    {
        Query.Where(x =>
                x.ElectionRoundId == electionRoundId &&
                x.MonitoringObserver.ObserverId == observerId &&
                x.MonitoringObserver.ElectionRoundId == electionRoundId)
            .Where(x => pollingStationIds.Contains(x.PollingStationId),
                pollingStationIds != null && pollingStationIds.Any());

        Query.Select(x => PollingStationInformationModel.FromEntity(x));
    }
}
