namespace Vote.Monitor.Feature.PollingStation.Specifications;

public class GetPollingStationByIdSpecification : Specification<PollingStationAggregate>,
    ISingleResultSpecification<PollingStationAggregate>
{
    public GetPollingStationByIdSpecification(Guid id)
    {
        Query.Where(pollingStation => pollingStation.Id == id);
    }
}
