using Ardalis.Specification;

namespace Vote.Monitor.Feature.PollingStation.Specifications;

public class GetPollingStationByIdSpecification : Specification<Domain.Entities.PollingStationAggregate.PollingStation>,
    ISingleResultSpecification<Domain.Entities.PollingStationAggregate.PollingStation>
{
    public GetPollingStationByIdSpecification(Guid id)
    {
        Query.Where(pollingStation => pollingStation.Id == id);
    }
}
