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
                  SELECT
                  EXISTS (
                    SELECT
                        1
                    FROM
                        "FormSubmissions" FS
                        INNER JOIN "MonitoringObservers" MO ON FS."MonitoringObserverId" = MO."Id"
                    WHERE
                        FS."ElectionRoundId" = @electionRoundId
                        AND MO."ObserverId" = @observerId
                        AND FS."PollingStationId" = @pollingStationId
                    UNION
                    SELECT
                        1
                    FROM
                        "QuickReports" QR
                        INNER JOIN "MonitoringObservers" MO ON QR."MonitoringObserverId" = MO."Id"
                    WHERE
                        QR."ElectionRoundId" = @electionRoundId
                        AND MO."ObserverId" = @observerId
                        AND QR."PollingStationId" = @pollingStationId
                  )
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
