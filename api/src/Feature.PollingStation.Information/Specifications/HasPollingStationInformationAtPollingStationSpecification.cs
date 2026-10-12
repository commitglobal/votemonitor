using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Feature.PollingStation.Information.Specifications;

public sealed class HasPollingStationInformationAtPollingStationSpecification : Specification<PollingStationInformation>
{
    public HasPollingStationInformationAtPollingStationSpecification(Guid electionRoundId, Guid pollingStationId,
        Guid monitoringObserverId, Guid excludedId)
    {
        Query.AsNoTracking();

        Query.Where(x => x.ElectionRoundId == electionRoundId
                         && x.PollingStationId == pollingStationId
                         && x.MonitoringObserverId == monitoringObserverId
                         && x.Id != excludedId);
    }
}
