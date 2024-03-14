using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.FormTemplate.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<FormTemplateAggregate> ApplyOrdering(this ISpecificationBuilder<FormTemplateAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(FormTemplateAggregate.Code), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting ? builder.OrderBy(x => x.Code) : builder.OrderByDescending(x => x.Code);
        }

        return builder.OrderBy(x => x.CreatedOn)
            .ThenBy(x => x.Code);
    }
}
