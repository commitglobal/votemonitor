using Vote.Monitor.Core.Models;

namespace Feature.Forms.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<FormAggregate> ApplyOrdering(this ISpecificationBuilder<FormAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(FormAggregate.Code), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting ? builder.OrderBy(x => x.Code) : builder.OrderByDescending(x => x.Code);
        }

        return builder.OrderBy(x => x.CreatedOn)
            .ThenBy(x => x.Code);
    }
}
