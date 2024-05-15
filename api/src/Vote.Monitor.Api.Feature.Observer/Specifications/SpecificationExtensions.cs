namespace Vote.Monitor.Api.Feature.Observer.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<ObserverAggregate> ApplyOrdering(this ISpecificationBuilder<ObserverAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(ApplicationUser.FirstName), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder
                    .OrderBy(x => x.ApplicationUser.FirstName)
                    .ThenBy(x => x.Id)
                : builder
                    .OrderByDescending(x => x.ApplicationUser.FirstName)
                    .ThenBy(x => x.Id);
        }
        
        if (string.Equals(request.SortColumnName, nameof(ApplicationUser.LastName), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder
                    .OrderBy(x => x.ApplicationUser.LastName)
                    .ThenBy(x => x.Id)
                : builder
                    .OrderByDescending(x => x.ApplicationUser.LastName)
                    .ThenBy(x => x.Id);
        }

        if (string.Equals(request.SortColumnName, nameof(ApplicationUser.Status), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder
                    .OrderBy(x => x.ApplicationUser.Status)
                    .ThenBy(x => x.Id)
                : builder
                    .OrderByDescending(x => x.ApplicationUser.Status)
                    .ThenBy(x => x.Id);
        }

        return builder.OrderBy(x => x.CreatedOn)
            .ThenBy(x => x.ApplicationUser.FirstName)
            .ThenBy(x => x.Id);
    }
}
