using Vote.Monitor.Domain.Specifications;

namespace Feature.NgoStaff.Specifications;

public sealed class ListNgoStaffSpecification : Specification<NgoStaffAggregate>
{
    public ListNgoStaffSpecification(List.Request request)
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
