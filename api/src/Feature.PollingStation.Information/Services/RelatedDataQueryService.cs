using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.PollingStation.Information.Services;

public class RelatedDataQueryService(INpgsqlConnectionFactory connectionFactory) : IRelatedDataQueryService
{
    public async Task<bool> GetHasDataForCurrentPollingStationAsync(Guid electionRoundId, Guid pollingStationId, Guid observerId,
        CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetOpenConnectionAsync(cancellationToken);
        var sql = """
                  SELECT "ObserverHasFormAnswersAtPollingStation"(@electionRoundId, @pollingStationId, @observerId)
                  """;

        var queryParams = new
        {
            electionRoundId,
            observerId,
            pollingStationId
        };

        return await connection.QuerySingleAsync<bool>(sql, queryParams);
    }
}
