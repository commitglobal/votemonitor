using System.Text.Json;
using System.Text.Json.Nodes;
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

    /// <summary>
    /// The parameter values as a JSON object, passed to a PL/pgSQL function together with a filter compiled with
    /// <see cref="FilterSqlCompiler.FunctionParameter"/>.
    /// </summary>
    public static string ToJsonParameters(this CompiledFilter filter)
    {
        var parameters = new JsonObject();
        foreach (var name in filter.Parameters.ParameterNames)
        {
            parameters[name] = JsonSerializer.SerializeToNode(filter.Parameters.Get<object?>(name));
        }

        return parameters.ToJsonString();
    }
}
