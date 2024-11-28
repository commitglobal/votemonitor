using Ardalis.Specification;

namespace Feature.Notes.Specifications;

public sealed class GetNotesSpecification : Specification<NoteAggregate>
{
    public GetNotesSpecification(Guid electionRoundId, Guid pollingStationId, Guid observerId, Guid formId)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId
                        && x.PollingStationId == pollingStationId
                        && x.MonitoringObserver.ObserverId == observerId
                        && x.MonitoringObserver.ElectionRoundId == electionRoundId
                        && x.Form.ElectionRoundId == electionRoundId
                        && x.FormId == formId);
    }
}
