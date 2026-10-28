using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Feature.PollingStation.Information.Forms.Specifications;

public sealed class CountNgoPollingStationInformationSpecification : Specification<PollingStationInformation>
{
    public CountNgoPollingStationInformationSpecification(Guid electionRoundId, Guid ngoId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId
                         && x.MonitoringObserver.MonitoringNgo.ElectionRoundId == electionRoundId
                         && x.MonitoringObserver.MonitoringNgo.NgoId == ngoId);
    }
}
