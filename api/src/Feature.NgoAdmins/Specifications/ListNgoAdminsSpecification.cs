using Vote.Monitor.Domain.Specifications;

namespace Feature.NgoAdmins.Specifications;

public sealed class ListNgoAdminsSpecification : Specification<NgoAdminAggregate>
{
    public ListNgoAdminsSpecification(List.Request request)
    {
        Query
            .Where(x => x.NgoId == request.NgoId)
            .Include(x => x.ApplicationUser)
            .Search(x => x.ApplicationUser.DisplayName, $"%{request.SearchText?.Trim() ?? string.Empty}%", !string.IsNullOrEmpty(request.SearchText))
            .Where(x => x.ApplicationUser.Status == request.Status, request.Status != null)
            .ApplyOrdering(request)
            .Paginate(request);
    }
}
