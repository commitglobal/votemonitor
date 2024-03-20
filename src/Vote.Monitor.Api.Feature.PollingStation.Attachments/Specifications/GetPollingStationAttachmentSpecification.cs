using Ardalis.Specification;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.Specifications;

public sealed class GetPollingStationAttachmentSpecification : Specification<PollingStationAttachmentAggregate>
{
    public GetPollingStationAttachmentSpecification(Guid electionRoundId,
        Guid pollingStationId,
        Guid observerId,
        Guid id)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId
                        && x.PollingStationId == pollingStationId
                        && x.MonitoringObserver.ObserverId == observerId
                        && x.Id == id
                        && x.IsDeleted == false);
    }
}
