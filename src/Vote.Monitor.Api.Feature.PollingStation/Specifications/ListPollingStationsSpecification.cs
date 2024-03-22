using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.PollingStation.Specifications;

public sealed class ListPollingStationsSpecification : Specification<PollingStationAggregate>
{
    public ListPollingStationsSpecification(List.Request request)
    {
        Query
            .Search(x => x.Address, "%" + request.AddressFilter + "%", !string.IsNullOrWhiteSpace(request.AddressFilter))
            .Where(station => EF.Functions.JsonContains(station.Tags, request.Filter),
                request.Filter is not null && request.Filter.Count != 0)
            .Where(x => x.ElectionRoundId == request.ElectionRoundId)
            .ApplyOrdering(request)
            .Paginate(request);
    }
}
