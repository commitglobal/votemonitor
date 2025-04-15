using Vote.Monitor.Core.Models;

namespace Feature.NgoAdmins.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<NgoAdminAggregate> ApplyOrdering(this ISpecificationBuilder<NgoAdminAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(ApplicationUser.FirstName), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting ? builder.OrderBy(x => x.ApplicationUser.FirstName) : builder.OrderByDescending(x => x.ApplicationUser.FirstName);
        }

        if (string.Equals(request.SortColumnName, nameof(ApplicationUser.LastName), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting ? builder.OrderBy(x => x.ApplicationUser.LastName) : builder.OrderByDescending(x => x.ApplicationUser.LastName);
        }

        return builder
            .OrderBy(x => x.CreatedOn)
            .ThenBy(x => x.ApplicationUser.FirstName);
    }
}
