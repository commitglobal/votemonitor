namespace Vote.Monitor.Api.Feature.PollingStation.Specifications;

public sealed class GetPollingStationSpecification : Specification<PollingStationAggregate>
{
    public GetPollingStationSpecification(string address, Dictionary<string, string> tags)
    {
        Query
            .Search(x => x.Address, "%" + address + "%", !string.IsNullOrEmpty(address))
            .Where(pollingStation => EF.Functions.JsonContains(pollingStation.Tags, tags), tags.Any());
    }
}
