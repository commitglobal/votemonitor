using Ardalis.Specification;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities;

namespace Vote.Monitor.Domain.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<T> Paginate<T>(this ISpecificationBuilder<T> builder, BaseFilterRequest filter) where T : class
    {
        return builder.Skip(PaginationHelper.CalculateSkip(filter.PageSize, filter.PageNumber))
            .Take(PaginationHelper.CalculateTake(filter.PageSize));
    }

    public static ISpecificationBuilder<T> ApplyDefaultOrdering<T>(this ISpecificationBuilder<T> builder,
        BaseFilterRequest filter) where T : AuditableBaseEntity
    {
        // We want the "asc" to be the default, that's why the condition is reverted.
        var isAscending = !(filter.SortOrder?.Equals(SortOrder.Asc) ?? false);

        if (string.Equals(filter.ColumnName, nameof(AuditableBaseEntity.CreatedOn), StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscending ? builder.OrderBy(x => x.CreatedOn) : builder.OrderByDescending(x => x.CreatedOn);
        }

        if (string.Equals(filter.ColumnName, nameof(AuditableBaseEntity.LastModifiedOn), StringComparison.InvariantCultureIgnoreCase))
        {
            return isAscending ? builder.OrderBy(x => x.LastModifiedOn) : builder.OrderByDescending(x => x.LastModifiedOn);
        }

        return builder.OrderBy(x => x.CreatedOn);
    }
}
