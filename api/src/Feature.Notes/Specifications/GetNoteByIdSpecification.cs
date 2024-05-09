using Ardalis.Specification;

namespace Feature.Notes.Specifications;

public sealed class GetNoteByIdSpecification : SingleResultSpecification<NoteAggregate>
{
    public GetNoteByIdSpecification(Guid electionRoundId, Guid observerId, Guid id)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId && x.MonitoringObserver.ObserverId == observerId && x.Id == id);
    }
}
