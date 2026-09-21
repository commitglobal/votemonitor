using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Feature.Statistics.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;

namespace Feature.Statistics.GetPollingStationStats;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFusionCache cache,
    IOptions<StatisticsFeatureOptions> options) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    private readonly StatisticsFeatureOptions _options = options.Value;

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}:stats");
        DontAutoTag();
        Options(x => x.WithTags("statistics", "polling-stations"));
        Summary(s => { s.Summary = "Overview statistics for a specific polling station"; });
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
            $"polling-station-stats-{req.ElectionRoundId}-{req.NgoId}-{req.PollingStationId}-{req.DataSource}";

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
                    SELECT COALESCE((
                        SELECT SUM(FS."NumberOfQuestionsAnswered")
                        FROM "FormSubmissions" FS
                        WHERE FS."ElectionRoundId" = @electionRoundId
                          AND FS."PollingStationId" = @pollingStationId
                          AND FS."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                    ), 0)
                    + COALESCE((
                        SELECT SUM(PSI."NumberOfQuestionsAnswered")
                        FROM "PollingStationInformation" PSI
                        WHERE PSI."ElectionRoundId" = @electionRoundId
                          AND PSI."PollingStationId" = @pollingStationId
                          AND PSI."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                    ), 0)
                ) AS "TotalAnswers",
                (
                    SELECT COALESCE((
                        SELECT SUM(FS."NumberOfFlaggedAnswers")
                        FROM "FormSubmissions" FS
                        WHERE FS."ElectionRoundId" = @electionRoundId
                          AND FS."PollingStationId" = @pollingStationId
                          AND FS."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                    ), 0)
                    + COALESCE((
                        SELECT SUM(PSI."NumberOfFlaggedAnswers")
                        FROM "PollingStationInformation" PSI
                        WHERE PSI."ElectionRoundId" = @electionRoundId
                          AND PSI."PollingStationId" = @pollingStationId
                          AND PSI."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                    ), 0)
                ) AS "FlaggedAnswers",
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
                        UNION
                        SELECT IR."MonitoringObserverId"
                        FROM "IncidentReports" IR
                        WHERE IR."ElectionRoundId" = @electionRoundId
                          AND IR."PollingStationId" = @pollingStationId
                          AND IR."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                    ) T
                ) AS "ObserversVisited",
                (
                    SELECT COALESCE(SUM("ComputeMinutesMonitoring"("ArrivalTime", "DepartureTime", "Breaks")), 0)
                    FROM "PollingStationInformation" PSI
                    WHERE PSI."ElectionRoundId" = @electionRoundId
                      AND PSI."PollingStationId" = @pollingStationId
                      AND PSI."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                ) AS "MinutesMonitoring",
                (
                    SELECT COUNT(*)
                    FROM "FormSubmissions" FS
                    WHERE FS."ElectionRoundId" = @electionRoundId
                      AND FS."PollingStationId" = @pollingStationId
                      AND FS."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                ) AS "FormSubmissions",
                (
                    SELECT COUNT(*)
                    FROM "QuickReports" QR
                    WHERE QR."ElectionRoundId" = @electionRoundId
                      AND QR."PollingStationId" = @pollingStationId
                      AND QR."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                ) AS "QuickReports",
                (
                    SELECT COUNT(*)
                    FROM "IncidentReports" IR
                    WHERE IR."ElectionRoundId" = @electionRoundId
                      AND IR."PollingStationId" = @pollingStationId
                      AND IR."MonitoringObserverId" IN (SELECT "MonitoringObserverId" FROM "AvailableObservers")
                ) AS "IncidentReports";
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
