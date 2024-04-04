namespace Vote.Monitor.Api.Feature.PollingStation.Specifications;

public sealed class GetPollingStationSpecification : Specification<PollingStationAggregate>
{
    public GetPollingStationSpecification(Guid electionRoundId, string address, Dictionary<string, string> tags)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId)
            .Search(x => x.Address, "%" + address + "%", !string.IsNullOrEmpty(address))
            .Where(x => EF.Functions.JsonContains(x.Tags, tags), tags.Any());
    }
}
