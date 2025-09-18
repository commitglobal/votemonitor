using Ardalis.Specification;

namespace Feature.Notes.Specifications;

public sealed class GetNotesV2Specification : Specification<NoteAggregate>
{
    public GetNotesV2Specification(Guid electionRoundId, Guid observerId, Guid submissionId)
    {
        Query
            .Where(x =>        
                        x.MonitoringObserver.ObserverId == observerId
                        && x.MonitoringObserver.ElectionRoundId == electionRoundId
                        && x.SubmissionId == submissionId);
    }
}
