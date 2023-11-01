using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;

namespace Vote.Monitor.Feature.PollingStation.Specifications;

public class GetPollingStationSpecification : Specification<Domain.Entities.PollingStationAggregate.PollingStation>
{
    public GetPollingStationSpecification(string address, Dictionary<string, string> tags)
    {
        Query.Search(x => x.Address, address)
            .Where(pollingStation => EF.Functions.JsonContains(pollingStation.Tags, tags));
    }
   
}
