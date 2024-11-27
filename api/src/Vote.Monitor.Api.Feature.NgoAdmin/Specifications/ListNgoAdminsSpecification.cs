using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.NgoAdmin.Specifications;

public sealed class ListNgoAdminsSpecification : Specification<NgoAdminAggregate>
{
    public ListNgoAdminsSpecification(List.Request request)
    {
        Query
            .Where(x => x.NgoId == request.NgoId)
            .Include(x => x.ApplicationUser)
            .Search(x => x.ApplicationUser.FirstName, $"%{request.SearchText?.Trim() ?? string.Empty}%", !string.IsNullOrEmpty(request.SearchText))
            .Search(x => x.ApplicationUser.LastName, $"%{request.SearchText?.Trim() ?? string.Empty}%", !string.IsNullOrEmpty(request.SearchText))
            .Where(x => x.ApplicationUser.Status == request.Status, request.Status != null)
            .ApplyOrdering(request)
            .Paginate(request);
    }
}
