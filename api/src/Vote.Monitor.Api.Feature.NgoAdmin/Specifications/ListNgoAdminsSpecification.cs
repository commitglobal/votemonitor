using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.NgoAdmin.Specifications;

public sealed class ListNgoAdminsSpecification : Specification<NgoAdminAggregate>
{
    public ListNgoAdminsSpecification(List.Request request)
    {
        Query
            .Where(x => x.NgoId == request.NgoId)
            .Include(x => x.ApplicationUser)
            .Search(x => x.ApplicationUser.FirstName, "%" + request.NameFilter + "%", !string.IsNullOrEmpty(request.NameFilter))
            .Search(x => x.ApplicationUser.LastName, "%" + request.NameFilter + "%", !string.IsNullOrEmpty(request.NameFilter))
            .Where(x => x.ApplicationUser.Status == request.Status, request.Status != null)
            .ApplyOrdering(request)
            .Paginate(request);
    }
}
