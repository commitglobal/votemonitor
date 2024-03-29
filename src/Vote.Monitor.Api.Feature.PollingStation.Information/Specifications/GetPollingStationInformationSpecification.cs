using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Specifications;

public sealed class GetPollingStationInformationSpecification : SingleResultSpecification<PollingStationInformation>
{
    public GetPollingStationInformationSpecification(Guid electionRoundId, Guid pollingStationId, Guid observerId)
    {
        Query.Where(x =>
            x.ElectionRoundId == electionRoundId && x.PollingStationId == pollingStationId &&
            x.MonitoringObserver.ObserverId == observerId);
    }
}
