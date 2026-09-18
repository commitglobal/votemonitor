using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Feature.Statistics.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;

namespace Feature.Statistics.GetPollingStationStatistics;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFusionCache cache,
    IOptions<StatisticsFeatureOptions> options) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    private readonly StatisticsFeatureOptions _options = options.Value;

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/statistics/polling-stations/{pollingStationId}");
        DontAutoTag();
        Options(x => x.WithTags("statistics"));
        Summary(s => { s.Summary = "Statistics for a specific polling station"; });
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var cacheKey =
            $"statistics-ps-{req.ElectionRoundId}-{req.NgoId}-{req.PollingStationId}-{req.DataSource}";

        var response = await cache.GetOrSetAsync(
            cacheKey,
            async _ => await GetStatisticsAsync(req, ct),
            cacheOptions => cacheOptions.SetDuration(TimeSpan.FromMinutes(_options.CacheDurationInMinutes)),
            token: ct);

        return TypedResults.Ok(response);
    }

    private async Task<Response> GetStatisticsAsync(Request req, CancellationToken ct)
    {
        const string sql =
            """
            WITH "AvailableObservers" AS (
                SELECT "MonitoringObserverId"
                FROM "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource)
            )
            SELECT
                (
                    SELECT COUNT(*)
                    FROM "FormSubmissions" FS
                    WHERE FS."ElectionRoundId" = @electionRoundId
                      AND FS."PollingStationId" = @pollingStationId
                      AND FS."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                ) AS "NumberOfFormSubmissions",
                (
                    SELECT COALESCE(SUM(FS."NumberOfQuestionsAnswered"), 0)
                    FROM "FormSubmissions" FS
                    WHERE FS."ElectionRoundId" = @electionRoundId
                      AND FS."PollingStationId" = @pollingStationId
                      AND FS."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                ) AS "NumberOfQuestionsAnswered",
                (
                    SELECT COUNT(*)
                    FROM "QuickReports" QR
                    WHERE QR."ElectionRoundId" = @electionRoundId
                      AND QR."PollingStationId" = @pollingStationId
                      AND QR."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                ) AS "NumberOfQuickReports",
                (
                    SELECT COUNT(DISTINCT "MonitoringObserverId")
                    FROM (
                        SELECT FS."MonitoringObserverId"
                        FROM "FormSubmissions" FS
                        WHERE FS."ElectionRoundId" = @electionRoundId
                          AND FS."PollingStationId" = @pollingStationId
                          AND FS."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                        UNION
                        SELECT PSI."MonitoringObserverId"
                        FROM "PollingStationInformation" PSI
                        WHERE PSI."ElectionRoundId" = @electionRoundId
                          AND PSI."PollingStationId" = @pollingStationId
                          AND PSI."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                        UNION
                        SELECT QR."MonitoringObserverId"
                        FROM "QuickReports" QR
                        WHERE QR."ElectionRoundId" = @electionRoundId
                          AND QR."PollingStationId" = @pollingStationId
                          AND QR."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                    ) T
                ) AS "NumberOfObservers",
                EXISTS (
                    SELECT 1
                    FROM "PollingStationInformation" PSI
                    WHERE PSI."ElectionRoundId" = @electionRoundId
                      AND PSI."PollingStationId" = @pollingStationId
                      AND PSI."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                ) AS "HasPollingStationInformation";
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            pollingStationId = req.PollingStationId,
            dataSource = req.DataSource.ToString()
        };

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        return await dbConnection.QueryFirstAsync<Response>(sql, queryArgs);
    }
}
