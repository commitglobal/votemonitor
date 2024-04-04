using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Specifications;

public sealed class GetPollingStationInformationForObserverSpecification : Specification<PollingStationInformation, PollingStationInformationModel>
{
    public GetPollingStationInformationForObserverSpecification(Guid electionRoundId, Guid observerId, List<Guid>? pollingStationIds)
    {
        Query.Where(x =>
            x.ElectionRoundId == electionRoundId &&
            x.MonitoringObserver.ObserverId == observerId)
            .Where(x => pollingStationIds.Contains(x.PollingStationId), pollingStationIds != null && pollingStationIds.Any());

        Query.Select(x => PollingStationInformationModel.FromEntity(x));

    }
}
