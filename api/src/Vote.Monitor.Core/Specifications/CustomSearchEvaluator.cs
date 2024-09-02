using Ardalis.Specification;

namespace Vote.Monitor.Core.Specifications;

internal class CustomSearchEvaluator : IEvaluator
{
    private CustomSearchEvaluator() { }
    public static CustomSearchEvaluator Instance { get; } = new();

    public bool IsCriteriaEvaluator { get; } = true;

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        foreach (var searchCriteria in specification.SearchCriterias.GroupBy(x => x.SearchGroup))
        {
            query = query.SearchIgnoreCase(searchCriteria);
        }

        return query;
    }
}
