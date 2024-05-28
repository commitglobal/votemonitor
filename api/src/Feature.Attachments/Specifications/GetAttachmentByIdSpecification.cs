using Ardalis.Specification;

namespace Feature.Attachments.Specifications;

public sealed class GetAttachmentByIdSpecification : SingleResultSpecification<AttachmentAggregate>
{
    public GetAttachmentByIdSpecification(Guid electionRoundId, Guid observerId, Guid attachmentId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId
                         && x.MonitoringObserver.ObserverId == observerId && x.Id == attachmentId);
    }
}
