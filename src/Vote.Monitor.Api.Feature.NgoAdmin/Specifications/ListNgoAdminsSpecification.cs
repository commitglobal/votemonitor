using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.NgoAdmin.Specifications;

public class ListNgoAdminsSpecification : Specification<NgoAdminAggregate>
{
    public ListNgoAdminsSpecification(List.Request request)
    {
        Query
            .Search(x => x.Name, "%" + request.NameFilter + "%", !string.IsNullOrEmpty(request.NameFilter))
            .Where(x => x.Status == request.Status, request.Status != null)
            .ApplyOrdering(request)
            .Paginate(request);
    }
}
