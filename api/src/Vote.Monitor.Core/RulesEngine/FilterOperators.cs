using Vote.Monitor.Core.RulesEngine.Rules;

namespace Vote.Monitor.Core.RulesEngine;

public static class FilterOperators
{
    public static readonly HashSet<string> Comparison =
    [
        FilterOps.Eq, FilterOps.Ne,
        FilterOps.Lt, FilterOps.Lte,
        FilterOps.Gt, FilterOps.Gte,
        FilterOps.Between
    ];

    public static readonly HashSet<string> Set =
    [
        FilterOps.Eq, FilterOps.Ne,
        FilterOps.In, FilterOps.NotIn,
        FilterOps.IsEmpty, FilterOps.IsNotEmpty
    ];

    public static readonly HashSet<string> Text =
    [
        FilterOps.Eq, FilterOps.Ne,
        FilterOps.Contains, FilterOps.NotContains,
        FilterOps.IsEmpty, FilterOps.IsNotEmpty
    ];

    // An array column, such as observer tags: "has any of" and "has none of".
    public static readonly HashSet<string> Array =
    [
        FilterOps.In, FilterOps.NotIn
    ];

    public static readonly HashSet<string> Boolean =
    [
        FilterOps.Eq, FilterOps.IsEmpty, FilterOps.IsNotEmpty
    ];
}
