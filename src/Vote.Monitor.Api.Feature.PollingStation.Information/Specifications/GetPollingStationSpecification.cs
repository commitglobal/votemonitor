using Ardalis.Specification;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Specifications;

public class GetPollingStationSpecification: SingleResultSpecification<Domain.Entities.PollingStationAggregate.PollingStation>
{
    public GetPollingStationSpecification(Guid electionRoundId, Guid pollingStationId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId && x.Id == pollingStationId);
    }
}
