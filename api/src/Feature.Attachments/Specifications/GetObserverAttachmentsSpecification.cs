using Ardalis.Specification;

namespace Feature.Attachments.Specifications;

public sealed class GetObserverAttachmentsSpecification : Specification<AttachmentAggregate>
{
    public GetObserverAttachmentsSpecification(Guid electionRoundId, Guid pollingStationId, Guid observerId, Guid formId)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId
                        && x.PollingStationId == pollingStationId
                        && x.MonitoringObserver.ObserverId == observerId
                        && x.FormId == formId
                        && x.IsDeleted == false);
    }
}
