namespace Vote.Monitor.Api.Feature.FormTemplate.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<FormTemplateAggregate> ApplyOrdering(this ISpecificationBuilder<FormTemplateAggregate> builder, BaseSortPaginatedRequest request)
    {
        if (string.Equals(request.SortColumnName, nameof(FormTemplateAggregate.Name), StringComparison.InvariantCultureIgnoreCase))
        {
            return request.IsAscendingSorting ? builder.OrderBy(x => x.Name) : builder.OrderByDescending(x => x.Name);
        }

        return builder.OrderBy(x => x.CreatedOn)
            .ThenBy(x => x.Name);
    }
}
