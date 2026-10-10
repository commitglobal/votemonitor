using Dapper;

namespace Vote.Monitor.Core.RulesEngine;

public static class CompiledFilterExtensions
{
    public static void AddTo(this CompiledFilter filter, DynamicParameters parameters)
    {
        foreach (var name in filter.Parameters.ParameterNames)
        {
            parameters.Add(name, filter.Parameters.Get<object>(name));
        }
    }
}
