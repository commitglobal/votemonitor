using Vote.Monitor.Core.Extensions;

namespace Vote.Monitor.Api.Feature.PollingStation.Specifications;

public class ListPollingStationsSpecification : Specification<PollingStationAggregate>
{
    public ListPollingStationsSpecification(List.Request request)
    {
        Query
            .Search(x => x.Address, "%" + request.AddressFilter + "%", !string.IsNullOrWhiteSpace(request.AddressFilter))
            .Where(station => EF.Functions.JsonContains(station.Tags, request.Filter),
                request.Filter is not null && request.Filter.Count != 0)
            .ApplyOrdering(request)
            .Paginate(request);
    }
}
