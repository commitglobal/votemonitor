using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Specifications;

public sealed class GetPollingStationInformationForObserverSpecification : Specification<PollingStationInformation, PollingStationInformationModel>
{
    public GetPollingStationInformationForObserverSpecification(Guid electionRoundId, Guid observerId)
    {
        Query.Where(x =>
            x.ElectionRoundId == electionRoundId &&
            x.MonitoringObserver.ObserverId == observerId);

        Query.Select(x => PollingStationInformationModel.FromEntity(x));

    }
}
