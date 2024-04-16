using Ardalis.Specification;
using Vote.Monitor.Core.Models;

namespace Feature.Form.Submissions.Specifications;

public static class SpecificationExtensions
{
    public static ISpecificationBuilder<FormSubmission> ApplyOrdering(this ISpecificationBuilder<FormSubmission> builder, BaseSortPaginatedRequest _)
    {
        return builder
            .OrderByDescending(x => x.CreatedOn)
            .ThenBy(x => x.Id);
    }
}
