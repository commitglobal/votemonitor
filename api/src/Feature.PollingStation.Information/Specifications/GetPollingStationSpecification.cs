using Ardalis.Specification;

namespace Feature.PollingStation.Information.Specifications;

public sealed class GetPollingStationSpecification: SingleResultSpecification<PollingStationAggregate>
{
    public GetPollingStationSpecification(Guid electionRoundId, Guid pollingStationId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId && x.Id == pollingStationId);
    }
}
