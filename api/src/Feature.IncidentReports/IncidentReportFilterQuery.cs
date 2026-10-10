using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.RulesEngine.Rules;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Queries;

namespace Feature.IncidentReports;

internal static class IncidentReportFilterQuery
{
    public static async Task<Guid[]> GetMatchingIdsAsync(
        INpgsqlConnectionFactory connectionFactory,
        Guid electionRoundId,
        Guid ngoId,
        FilterRule? conditions,
        CancellationToken ct)
    {
        var filter = new FilterSqlCompiler(V2ReportFilterFields.IncidentReports).Build(conditions);
        var parameters = new DynamicParameters();
        parameters.Add("electionRoundId", electionRoundId);
        parameters.Add("ngoId", ngoId);
        parameters.Add("dataSource", DataSource.Ngo.ToString());
        filter.AddTo(parameters);

        const string sql = """
                           SELECT s."Id"
                           FROM "GetIncidentReportEntries"(@electionRoundId, @ngoId, @dataSource) s
                           WHERE s."ElectionRoundId" = @electionRoundId
                             AND s."NgoId" = @ngoId
                             AND
                           """;
        using var connection = await connectionFactory.GetOpenConnectionAsync(ct);
        var ids = await connection.QueryAsync<Guid>($"{sql} ({filter.Sql})", parameters);
        return ids.ToArray();
    }
}
