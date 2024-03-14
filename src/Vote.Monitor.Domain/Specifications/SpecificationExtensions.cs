using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<T> Paginate<T>(this ISpecificationBuilder<T> builder, BasePaginatedRequest request) where T : class
    {
        return builder.Skip(PaginationHelper.CalculateSkip(request.PageSize, request.PageNumber))
            .Take(PaginationHelper.CalculateTake(request.PageSize));
    }

    public static ISpecificationBuilder<T> Paginate<T>(this ISpecificationBuilder<T> builder, BaseSortPaginatedRequest request) where T : class
    {
        return builder.Skip(PaginationHelper.CalculateSkip(request.PageSize, request.PageNumber))
            .Take(PaginationHelper.CalculateTake(request.PageSize));
    }

    public static ISpecificationBuilder<T> ApplyDefaultOrdering<T>(this ISpecificationBuilder<T> builder,
        BaseSortPaginatedRequest request) where T : AuditableBaseEntity
    {
        if (string.Equals(request.SortColumnName, nameof(AuditableBaseEntity.CreatedOn), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder
                    .OrderBy(x => x.CreatedOn)
                    .ThenBy(x => x.Id)
                : builder
                    .OrderByDescending(x => x.CreatedOn)
                    .ThenBy(x => x.Id);
        }

        if (string.Equals(request.SortColumnName, nameof(AuditableBaseEntity.LastModifiedOn), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting
                ? builder
                    .OrderBy(x => x.LastModifiedOn)
                    .ThenBy(x => x.Id)
                : builder
                    .OrderByDescending(x => x.LastModifiedOn)
                    .ThenBy(x => x.Id);
        }

        return builder
            .OrderBy(x => x.CreatedOn)
            .ThenBy(x => x.Id);
    }
}
