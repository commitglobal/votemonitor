using Dapper;

namespace Vote.Monitor.Core.RulesEngine;

public sealed record CompiledFilter(string Sql, DynamicParameters Parameters);
