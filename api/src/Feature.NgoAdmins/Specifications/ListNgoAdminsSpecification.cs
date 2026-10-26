using Vote.Monitor.Domain.Specifications;

namespace Feature.NgoAdmins.Specifications;

public sealed class ListNgoAdminsSpecification : Specification<NgoAdminAggregate>
{
    public ListNgoAdminsSpecification(List.Request request)
    {
        Query.AsNoTracking();

        Query
            .Where(x => x.NgoId == request.NgoId)
            .Include(x => x.ApplicationUser)
            .Search(x => x.ApplicationUser.DisplayName, $"%{request.SearchText?.Trim() ?? string.Empty}%", !string.IsNullOrEmpty(request.SearchText))
            .Where(x => request.Status!.Contains(x.ApplicationUser.Status), request.Status is { Length: > 0 })
            .ApplyOrdering(request)
            .Paginate(request);
    }
}
