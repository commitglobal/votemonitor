using Vote.Monitor.Domain.Specifications;

namespace Feature.PollingStations.Specifications;

public sealed class ListPollingStationsSpecification : Specification<PollingStationAggregate>
{
    public ListPollingStationsSpecification(List.Request request)
    {
        var level1 = NonBlank(request.Level1Filter);
        var level2 = NonBlank(request.Level2Filter);
        var level3 = NonBlank(request.Level3Filter);
        var level4 = NonBlank(request.Level4Filter);
        var level5 = NonBlank(request.Level5Filter);

        Query
            .Search(x => x.Address, "%" + request.AddressFilter + "%", !string.IsNullOrWhiteSpace(request.AddressFilter))
            .Where(x => x.ElectionRoundId == request.ElectionRoundId)
            .Where(x => level1.Contains(x.Level1), level1.Length > 0)
            .Where(x => level2.Contains(x.Level2), level2.Length > 0)
            .Where(x => level3.Contains(x.Level3), level3.Length > 0)
            .Where(x => level4.Contains(x.Level4), level4.Length > 0)
            .Where(x => level5.Contains(x.Level5), level5.Length > 0)
            // Part of the number ("01/" finds 01/001, 01/002…); its own search group, so it is ANDed with the address.
            .Search(x => x.Number, "%" + request.PollingStationNumberFilter + "%", !string.IsNullOrWhiteSpace(request.PollingStationNumberFilter), searchGroup: 2)
            .ApplyOrdering(request)
            .Paginate(request)
            .AsNoTracking();
    }

    private static string[] NonBlank(string[]? values) =>
        values?.Where(value => !string.IsNullOrWhiteSpace(value)).ToArray() ?? [];
}
