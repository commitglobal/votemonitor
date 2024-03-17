using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Specifications;

public sealed class GetPollingStationInformationByIdSpecification : Specification<PollingStationInformation>
{
    public GetPollingStationInformationByIdSpecification(Guid electionRoundId, Guid pollingStationId, Guid observerId, Guid pollingInformationId)
    {
        Query.Where(x =>
            x.Id == pollingInformationId
            && x.ElectionRoundId == electionRoundId
            && x.PollingStationId == pollingStationId
            && x.MonitoringObserver.ObserverId == observerId);

    }
}
