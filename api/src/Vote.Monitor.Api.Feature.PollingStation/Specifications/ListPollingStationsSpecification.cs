using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.PollingStation.Specifications;

public sealed class ListPollingStationsSpecification : Specification<PollingStationAggregate>
{
    public ListPollingStationsSpecification(List.Request request)
    {
        Query
            .Search(x => x.Address, "%" + request.AddressFilter + "%", !string.IsNullOrWhiteSpace(request.AddressFilter))
            .Where(x => x.ElectionRoundId == request.ElectionRoundId)
            .ApplyOrdering(request)
            .Paginate(request);
    }
}
