using Dapper;
using Feature.Statistics.Options;
using Microsoft.Extensions.Options;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;
using Vote.Monitor.Core.Models;

namespace Feature.Statistics.GetPlatformStatistics;

public class Endpoint(
    INpgsqlConnectionFactory dbConnectionFactory,
    IFusionCache cache,
    IOptions<StatisticsFeatureOptions> options) : Endpoint<Request, Response>
{
    private readonly StatisticsFeatureOptions _options = options.Value;

    public override void Configure()
    {
        Get("/api/statistics/overview");
        AllowAnonymous();
        DontAutoTag();
        Options(x => x.WithTags("statistics"));
        Summary(s =>
        {
            s.Summary = "Statistics for an list of election round";
        });
    }

    public override async Task<Response> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRoundKey = string.Join(",", (req.ElectionRoundIds ?? []).OrderBy(x => x));
        var cacheKey = $"statistics-overview-{electionRoundKey}";

        var cacheDuration = TimeSpan.FromMinutes(_options.CacheDurationInMinutes);

        return await cache.GetOrSetAsync(
            cacheKey,
            CachedResponse.Track(async _ => await GetOverviewStatisticsAsync(req, ct), cacheDuration),
            cacheOptions => cacheOptions.SetDuration(cacheDuration),
            tags: [CacheTags.Statistics, ..(req.ElectionRoundIds ?? []).Select(CacheTags.ElectionRound)],
            token: ct);
    }

    private async Task<Response> GetOverviewStatisticsAsync(Request req, CancellationToken ct)
    {
        var statisticsQuery = """
                              SELECT "GetPlatformStatisticsMonitoringObservers"(@electionRoundIds);
                              SELECT "GetPlatformStatisticsMonitoringNgos"(@electionRoundIds);
                              SELECT "GetPlatformStatisticsPollingStations"(@electionRoundIds);
                              SELECT * FROM "GetPlatformStatisticsPollingStationVisits"(@electionRoundIds);
                              SELECT "GetPlatformStatisticsFormSubmissions"(@electionRoundIds);
                              SELECT "GetPlatformStatisticsQuestionsAnswered"(@electionRoundIds);
                              SELECT "GetPlatformStatisticsFlaggedAnswers"(@electionRoundIds);
                              SELECT "GetPlatformStatisticsMinutesMonitoring"(@electionRoundIds);
                              """;

        var queryArgs = new
        {
            electionRoundIds = req.ElectionRoundIds
        };

        int numberOfMonitoringObservers;
        int numberOfMonitoringNgos;
        int totalNumberOfPollingStations;
        PollingStationVisitsView pollingStationVisitsView;
        int numberOfSubmittedForms;
        int numberOfAnsweredQuestions;
        int numberOfFlaggedAnswers;
        int minutesMonitoring;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(statisticsQuery, queryArgs);

            numberOfMonitoringObservers = multi.ReadSingle<int>();
            numberOfMonitoringNgos = multi.ReadSingle<int>();
            totalNumberOfPollingStations = multi.ReadSingle<int>();
            pollingStationVisitsView = multi.ReadSingle<PollingStationVisitsView>();
            numberOfSubmittedForms = multi.ReadSingle<int>();
            numberOfAnsweredQuestions = multi.ReadSingle<int>();
            numberOfFlaggedAnswers = multi.ReadSingle<int>();
            minutesMonitoring = multi.ReadSingle<int>();
        }

        return new Response
        {
            Observers = numberOfMonitoringObservers,
            Ngos = numberOfMonitoringNgos,
            PollingStations = totalNumberOfPollingStations,
            VisitedPollingStations = pollingStationVisitsView?.NumberOfVisitedPollingStations ?? 0,
            Level1Visited = pollingStationVisitsView?.NumberOfLevel1Covered ?? 0,
            Level2Visited = pollingStationVisitsView?.NumberOfLevel2Covered ?? 0,
            Level3Visited = pollingStationVisitsView?.NumberOfLevel3Covered ?? 0,
            Level4Visited = pollingStationVisitsView?.NumberOfLevel4Covered ?? 0,
            Level5Visited = pollingStationVisitsView?.NumberOfLevel5Covered ?? 0,
            StartedForms = numberOfSubmittedForms,
            QuestionsAnswered = numberOfAnsweredQuestions,
            FlaggedAnswers = numberOfFlaggedAnswers,
            MinutesMonitoring = minutesMonitoring
        };
    }
}
