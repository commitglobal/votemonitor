using Authorization.Policies;
using Dapper;
using Feature.Statistics.Options;
using Microsoft.Extensions.Options;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;

namespace Feature.Statistics.GetPlatformAdminStatistics;

public class Endpoint(
    INpgsqlConnectionFactory dbConnectionFactory,
    IFusionCache cache,
    IOptions<StatisticsFeatureOptions> options) : EndpointWithoutRequest<Response>
{
    private const string CacheKey = "statistics-platform-admin";
    private readonly StatisticsFeatureOptions _options = options.Value;

    public override void Configure()
    {
        Get("/api/statistics/platform");
        DontAutoTag();
        Options(x => x.WithTags("statistics"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Platform-wide statistics for platform admins";
        });
    }

    public override async Task<Response> ExecuteAsync(CancellationToken ct)
    {
        return await cache.GetOrSetAsync(
            CacheKey,
            async _ => await GetPlatformStatisticsAsync(ct),
            cacheOptions => cacheOptions.SetDuration(TimeSpan.FromMinutes(_options.CacheDurationInMinutes)),
            token: ct);
    }

    private async Task<Response> GetPlatformStatisticsAsync(CancellationToken ct)
    {
        const string sql =
            """
            SELECT
                C."Id" AS "CountryId",
                C."Name" AS "CountryName",
                C."Iso2" AS "Iso2",
                COUNT(ER."Id") AS "NumberOfElections"
            FROM "ElectionRounds" ER
                INNER JOIN "Countries" C ON C."Id" = ER."CountryId"
            GROUP BY C."Id", C."Name", C."Iso2"
            ORDER BY C."Name";
            ------------------------------

            SELECT
                COUNT(*) FILTER (WHERE U."Status" = 'Active') AS "ActiveObservers",
                COUNT(*) FILTER (WHERE U."Status" = 'Deactivated') AS "SuspendedObservers"
            FROM "Observers" O
                INNER JOIN "AspNetUsers" U ON U."Id" = O."Id";
            ------------------------------

            SELECT
                COUNT(*) FILTER (WHERE "Status" = 'Activated') AS "ActiveNgos",
                COUNT(*) FILTER (WHERE "Status" = 'Deactivated') AS "DeactivatedNgos"
            FROM "Ngos";
            ------------------------------

            SELECT COUNT(*) FROM "ElectionRounds";
            ------------------------------

            SELECT COUNT(*) FROM "PollingStations";
            ------------------------------

            SELECT COUNT(DISTINCT T."PollingStationId")
            FROM (
                SELECT FS."PollingStationId"
                FROM "FormSubmissions" FS
                    INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                WHERE F."Status" <> 'Drafted'
                  AND FS."NumberOfQuestionsAnswered" > 0
                UNION
                SELECT PSI."PollingStationId"
                FROM "PollingStationInformation" PSI
                WHERE PSI."NumberOfQuestionsAnswered" > 0
            ) AS T;
            ------------------------------

            SELECT COALESCE(SUM("ComputeMinutesMonitoring"("ArrivalTime", "DepartureTime", "Breaks")), 0)
            FROM "PollingStationInformation";
            ------------------------------

            SELECT
                (
                    SELECT COUNT(1)
                    FROM "FormSubmissions" FS
                        INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                    WHERE FS."NumberOfQuestionsAnswered" > 0
                      AND F."Status" <> 'Drafted'
                ) + (
                    SELECT COUNT(1)
                    FROM "PollingStationInformation"
                    WHERE "NumberOfQuestionsAnswered" > 0
                );
            ------------------------------

            SELECT
                (
                    SELECT COALESCE(SUM("NumberOfQuestionsAnswered"), 0)
                    FROM "FormSubmissions" FS
                        INNER JOIN "Forms" F ON F."Id" = FS."FormId"
                    WHERE FS."NumberOfQuestionsAnswered" > 0
                      AND F."Status" <> 'Drafted'
                ) + (
                    SELECT COALESCE(SUM("NumberOfQuestionsAnswered"), 0)
                    FROM "PollingStationInformation"
                );
            ------------------------------

            SELECT COALESCE(SUM(FS."NumberOfFlaggedAnswers"), 0)
            FROM "FormSubmissions" FS
                INNER JOIN "Forms" F ON F."Id" = FS."FormId"
            WHERE FS."NumberOfQuestionsAnswered" > 0
              AND F."Status" <> 'Drafted';
            ------------------------------

            SELECT COUNT(*) FROM "QuickReports";
            ------------------------------

            SELECT COUNT(*) FROM "IncidentReports";
            ------------------------------

            SELECT COUNT(*) FROM "CitizenReports";
            """;

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        using var multi = await dbConnection.QueryMultipleAsync(sql);

        var countriesHistogram = multi.Read<CountryHistogramPoint>().ToList();
        var observers = multi.ReadSingle<ObserversStats>();
        var ngos = multi.ReadSingle<NgosStats>();
        var numberOfElections = multi.ReadSingle<int>();
        var numberOfPollingStations = multi.ReadSingle<int>();
        var numberOfVisitedPollingStations = multi.ReadSingle<int>();
        var numberOfMinutesMonitoring = multi.ReadSingle<int>();
        var numberOfFormSubmissions = multi.ReadSingle<int>();
        var numberOfQuestionsAnswered = multi.ReadSingle<int>();
        var numberOfFlaggedAnswers = multi.ReadSingle<int>();
        var numberOfQuickReports = multi.ReadSingle<int>();
        var numberOfIncidentReports = multi.ReadSingle<int>();
        var numberOfCitizenReports = multi.ReadSingle<int>();

        return new Response
        {
            CountriesHistogram = countriesHistogram,
            Observers = observers,
            Ngos = ngos,
            NumberOfElections = numberOfElections,
            NumberOfPollingStations = numberOfPollingStations,
            NumberOfVisitedPollingStations = numberOfVisitedPollingStations,
            NumberOfMinutesMonitoring = numberOfMinutesMonitoring,
            NumberOfFormSubmissions = numberOfFormSubmissions,
            NumberOfQuestionsAnswered = numberOfQuestionsAnswered,
            NumberOfFlaggedAnswers = numberOfFlaggedAnswers,
            NumberOfQuickReports = numberOfQuickReports,
            NumberOfIncidentReports = numberOfIncidentReports,
            NumberOfCitizenReports = numberOfCitizenReports
        };
    }
}
