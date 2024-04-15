using Ardalis.Specification;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes.Specifications;

public sealed class GetPollingStationSpecification : Specification<PollingStationAggregate>
{
    public GetPollingStationSpecification(Guid pollingStationId)
    {
        Query.Where(p => p.Id == pollingStationId);
    }
}
