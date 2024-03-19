using Ardalis.Specification;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.Specifications;

public sealed class GetPollingStationAttachmentsSpecification : Specification<PollingStationAttachmentAggregate>
{
    public GetPollingStationAttachmentsSpecification(Guid electionRoundId, Guid pollingStationId, Guid observerId)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId
                        && x.PollingStationId == pollingStationId
                        && x.MonitoringObserver.ObserverId == observerId
                        && x.IsDeleted == false);
    }
}
