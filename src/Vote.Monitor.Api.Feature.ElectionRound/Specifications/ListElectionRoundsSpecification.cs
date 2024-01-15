using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.ElectionRound.Specifications;

public class ListElectionRoundsSpecification : Specification<ElectionRoundAggregate>
{
    public ListElectionRoundsSpecification(List.Request request)
    {
        Query
            .Search(x => x.Name, "%" + request.NameFilter + "%", !string.IsNullOrEmpty(request.NameFilter))
            .Where(x => x.Status == request.Status, request.Status != null)
            .Where(x => x.CountryId == request.CountryId, request.CountryId != null)
            .ApplyOrdering(request)
            .Paginate(request);
    }
}
