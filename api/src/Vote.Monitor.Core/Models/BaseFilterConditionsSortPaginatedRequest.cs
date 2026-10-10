using Vote.Monitor.Core.RulesEngine.Rules;

namespace Vote.Monitor.Core.Models;

public class BaseFilterConditionsSortPaginatedRequest : BaseSortPaginatedBodyRequest
{
    public FilterRule? FilterConditions { get; set; }
}
