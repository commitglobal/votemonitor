using Authorization.Policies;
using Dapper;
using Feature.Statistics.Options;
using Microsoft.Extensions.Options;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;
using Vote.Monitor.Core.Models;

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
        var cacheDuration = TimeSpan.FromMinutes(_options.CacheDurationInMinutes);

        return await cache.GetOrSetAsync(
            CacheKey,
            CachedResponse.Track(async _ => await GetPlatformStatisticsAsync(ct), cacheDuration),
            cacheOptions => cacheOptions.SetDuration(cacheDuration),
            tags: [CacheTags.Statistics],
            token: ct);
    }

    private async Task<Response> GetPlatformStatisticsAsync(CancellationToken ct)
    {
        const string sql =
            """
            SELECT * FROM "GetPlatformAdminStatisticsCountriesHistogram"();
            SELECT * FROM "GetPlatformAdminStatisticsObservers"();
            SELECT * FROM "GetPlatformAdminStatisticsNgos"();
            SELECT "GetPlatformAdminStatisticsElectionRounds"();
            SELECT "GetPlatformAdminStatisticsPollingStations"();
            SELECT "GetPlatformAdminStatisticsVisitedPollingStations"();
            SELECT "GetPlatformAdminStatisticsMinutesMonitoring"();
            SELECT "GetPlatformAdminStatisticsFormSubmissions"();
            SELECT "GetPlatformAdminStatisticsQuestionsAnswered"();
            SELECT "GetPlatformAdminStatisticsFlaggedAnswers"();
            SELECT "GetPlatformAdminStatisticsQuickReports"();
            SELECT "GetPlatformAdminStatisticsIncidentReports"();
            SELECT "GetPlatformAdminStatisticsCitizenReports"();
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
