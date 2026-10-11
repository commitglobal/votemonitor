using Ardalis.Specification;

namespace Feature.Notes.Specifications;

public sealed class GetNoteByIdReadOnlySpecification : SingleResultSpecification<NoteAggregate>
{
    [Obsolete("Will be removed in future version.")]
    public GetNoteByIdReadOnlySpecification(Guid electionRoundId, Guid observerId, Guid id)
    {
        Query.AsNoTracking();

        Query
            .Where(x => x.ElectionRoundId == electionRoundId
                        && x.MonitoringObserver.ObserverId == observerId
                        && x.Id == id
                        && x.MonitoringObserver.ElectionRoundId == electionRoundId);
    }

    public GetNoteByIdReadOnlySpecification(Guid electionRoundId, Guid submissionId, Guid observerId, Guid id)
    {
        Query.AsNoTracking();

        Query
            .Where(x => x.SubmissionId == submissionId
                        && x.MonitoringObserver.ObserverId == observerId
                        && x.Id == id
                        && x.MonitoringObserver.ElectionRoundId == electionRoundId);
    }
}
