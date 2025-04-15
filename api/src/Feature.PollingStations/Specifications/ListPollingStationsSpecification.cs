using Vote.Monitor.Domain.Specifications;

namespace Feature.PollingStations.Specifications;

public sealed class ListPollingStationsSpecification : Specification<PollingStationAggregate>
{
    public ListPollingStationsSpecification(List.Request request)
    {
        Query
            .Search(x => x.Address, "%" + request.AddressFilter + "%", !string.IsNullOrWhiteSpace(request.AddressFilter))
            .Where(x => x.ElectionRoundId == request.ElectionRoundId)
            .Where(x => x.Level1 == request.Level1Filter, !string.IsNullOrWhiteSpace(request.Level1Filter))
            .Where(x => x.Level2 == request.Level2Filter, !string.IsNullOrWhiteSpace(request.Level2Filter))
            .Where(x => x.Level3 == request.Level3Filter, !string.IsNullOrWhiteSpace(request.Level3Filter))
            .Where(x => x.Level4 == request.Level4Filter, !string.IsNullOrWhiteSpace(request.Level4Filter))
            .Where(x => x.Level5 == request.Level5Filter, !string.IsNullOrWhiteSpace(request.Level5Filter))
            .Where(x => x.Number == request.PollingStationNumberFilter, !string.IsNullOrWhiteSpace(request.PollingStationNumberFilter))
            .ApplyOrdering(request)
            .Paginate(request)
            .AsNoTracking();
    }
}
