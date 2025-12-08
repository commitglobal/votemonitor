using Ardalis.Specification;

namespace Feature.Attachments.Specifications;

public sealed class GetObserverAttachmentsSpecification : Specification<AttachmentAggregate>
{
    [Obsolete("Will be removed in future version")]

    public GetObserverAttachmentsSpecification(Guid electionRoundId, Guid pollingStationId, Guid observerId, Guid formId)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId
                        && x.PollingStationId == pollingStationId
                        && x.MonitoringObserver.ObserverId == observerId
                        && x.MonitoringObserver.ElectionRoundId == electionRoundId
                        && x.FormId == formId
                        && x.IsDeleted == false
                        && x.IsCompleted == true);
    }   
    
    public GetObserverAttachmentsSpecification(Guid electionRoundId, Guid submissionId, Guid observerId)
    {
        Query
            .Where(x => 
                           x.SubmissionId == submissionId
                        && x.MonitoringObserver.ObserverId == observerId
                        && x.MonitoringObserver.ElectionRoundId == electionRoundId
                        && x.IsDeleted == false
                        && x.IsCompleted == true);
    }
}
