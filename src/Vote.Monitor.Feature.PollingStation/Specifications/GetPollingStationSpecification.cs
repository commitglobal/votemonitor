namespace Vote.Monitor.Feature.PollingStation.Specifications;

public class GetPollingStationSpecification : Specification<PollingStationAggregate>
{
    public GetPollingStationSpecification(string address, Dictionary<string, string> tags)
    {
        Query.Search(x => x.Address, address)
            .Where(pollingStation => EF.Functions.JsonContains(pollingStation.Tags, tags), tags.Any());
    }
}
