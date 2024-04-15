using Ardalis.Specification;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes.Specifications;

public sealed class GetPollingStationNotesSpecification : Specification<PollingStationNoteAggregate>
{
    public GetPollingStationNotesSpecification(Guid electionRoundId, Guid pollingStationId, Guid observerId)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId
                        && x.PollingStationId == pollingStationId
                        && x.MonitoringObserver.ObserverId == observerId);
    }
}
