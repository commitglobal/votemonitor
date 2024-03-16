using Ardalis.Specification;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes.Specifications;

public class GetPollingStationNoteSpecification : Specification<PollingStationNoteAggregate>
{
    public GetPollingStationNoteSpecification(Guid electionRoundId, Guid pollingStationId, Guid observerId, Guid id)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId
                    && x.PollingStationId == pollingStationId
                    && x.MonitoringObserver.ObserverId == observerId
                    && x.Id == id);
    }
}
