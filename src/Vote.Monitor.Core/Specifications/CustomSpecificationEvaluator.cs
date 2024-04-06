using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;

namespace Vote.Monitor.Core.Specifications;

public class CustomSpecificationEvaluator : SpecificationEvaluator
{
    public static CustomSpecificationEvaluator Instance { get; } = new ();

    private CustomSpecificationEvaluator()
        : base(new IEvaluator[] {
            WhereEvaluator.Instance,
            CustomSearchEvaluator.Instance,
            IncludeEvaluator.Default,
            OrderEvaluator.Instance,
            PaginationEvaluator.Instance,
            AsNoTrackingEvaluator.Instance,
            IgnoreQueryFiltersEvaluator.Instance,
            AsSplitQueryEvaluator.Instance,
            AsNoTrackingWithIdentityResolutionEvaluator.Instance
        })
    {
    }
}
