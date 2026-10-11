using Ardalis.Specification;

namespace Feature.Attachments.Specifications;

public sealed class GetAttachmentByIdReadOnlySpecification : SingleResultSpecification<AttachmentAggregate>
{
    public GetAttachmentByIdReadOnlySpecification(Guid electionRoundId, Guid observerId, Guid attachmentId)
    {
        Query.AsNoTracking();

        Query.Where(x => x.MonitoringObserver.ElectionRoundId == electionRoundId
                         && x.MonitoringObserver.ObserverId == observerId 
                         && x.Id == attachmentId);
    }
}
