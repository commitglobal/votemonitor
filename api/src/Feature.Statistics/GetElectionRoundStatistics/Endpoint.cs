using Authorization.Policies;
using Dapper;
using Feature.Statistics.GetNgoAdminStatistics.Models;
using Feature.Statistics.Options;
using Microsoft.Extensions.Options;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;
using Vote.Monitor.Core.Models;

namespace Feature.Statistics.GetElectionRoundStatistics;

public class Endpoint(
    INpgsqlConnectionFactory dbConnectionFactory,
    IFusionCache cache,
    IOptions<StatisticsFeatureOptions> options) : Endpoint<Request, Response>
{
    private readonly StatisticsFeatureOptions _options = options.Value;

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}:stats");
        DontAutoTag();
        Options(x => x.WithTags("statistics"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Statistics for an election round across all monitoring NGOs";
        });
    }

    public override async Task<Response> ExecuteAsync(Request req, CancellationToken ct)
    {
        var cacheKey = $"statistics-election-round-{req.ElectionRoundId}";

        var cacheDuration = TimeSpan.FromMinutes(_options.CacheDurationInMinutes);

        return await cache.GetOrSetAsync(
            cacheKey,
            CachedResponse.Track(async _ => await GetStatisticsAsync(req, ct), cacheDuration),
            cacheOptions => cacheOptions.SetDuration(cacheDuration),
            tags: [CacheTags.Statistics, CacheTags.ElectionRound(req.ElectionRoundId)],
            token: ct);
    }

    private async Task<Response> GetStatisticsAsync(Request req, CancellationToken ct)
    {
        const string sql =
            """
            SELECT * FROM "StatsOverviewNgosCounts"(@electionRoundId);
            SELECT "StatsOverviewCoalitionsCount"(@electionRoundId);
            SELECT * FROM "StatsOverviewObservers"(@electionRoundId, NULL, NULL);
            SELECT * FROM "StatsOverviewPollingStationLevels"(@electionRoundId, NULL, NULL);
            SELECT * FROM "StatsOverviewLocationLevels"(@electionRoundId, NULL, NULL);
            SELECT * FROM "StatsOverviewFormsHistogram"(@electionRoundId, NULL, NULL) ORDER BY "Bucket";
            SELECT * FROM "StatsOverviewQuickReportsHistogram"(@electionRoundId, NULL, NULL);
            SELECT * FROM "StatsOverviewCitizenReportsHistogram"(@electionRoundId, NULL, NULL);
            SELECT * FROM "StatsOverviewIncidentReportsHistogram"(@electionRoundId, NULL, NULL);
            """;

        var queryArgs = new { electionRoundId = req.ElectionRoundId };

        NgosStats ngosStats;
        int numberOfCoalitions;
        ObserversStats observersStats;
        List<VisitedPollingStationLevelStats> visitedPollingStationsStats;
        List<VisitedLocationLevelStats> visitedLocationStats;
        List<FormSubmissionsHistogramPoint> formSubmissionsHistogram;
        List<HistogramPoint> quickReportsHistogram;
        List<HistogramPoint> citizenReportsHistogram;
        List<HistogramPoint> incidentReportsHistogram;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);

            ngosStats = multi.ReadSingle<NgosStats>();
            numberOfCoalitions = multi.ReadSingle<int>();
            observersStats = multi.ReadSingle<ObserversStats>();
            visitedPollingStationsStats = multi.Read<VisitedPollingStationLevelStats>().ToList();
            visitedLocationStats = multi.Read<VisitedLocationLevelStats>().ToList();
            formSubmissionsHistogram = multi.Read<FormSubmissionsHistogramPoint>().ToList();
            quickReportsHistogram = multi.Read<HistogramPoint>().ToList();
            citizenReportsHistogram = multi.Read<HistogramPoint>().ToList();
            incidentReportsHistogram = multi.Read<HistogramPoint>().ToList();
        }

        return new Response
        {
            NgosStats = ngosStats,
            NumberOfCoalitions = numberOfCoalitions,
            ObserversStats = observersStats,
            TotalStats = visitedPollingStationsStats.FirstOrDefault(x => x.Level == 0),
            Level1Stats = visitedPollingStationsStats.Where(x => x.Level == 1).ToList(),
            Level2Stats = visitedPollingStationsStats.Where(x => x.Level == 2).ToList(),
            Level3Stats = visitedPollingStationsStats.Where(x => x.Level == 3).ToList(),
            Level4Stats = visitedPollingStationsStats.Where(x => x.Level == 4).ToList(),
            Level5Stats = visitedPollingStationsStats.Where(x => x.Level == 5).ToList(),
            TotalLocationStats = visitedLocationStats.FirstOrDefault(x => x.Level == 0),
            LocationLevel1Stats = visitedLocationStats.Where(x => x.Level == 1).ToList(),
            LocationLevel2Stats = visitedLocationStats.Where(x => x.Level == 2).ToList(),
            LocationLevel3Stats = visitedLocationStats.Where(x => x.Level == 3).ToList(),
            LocationLevel4Stats = visitedLocationStats.Where(x => x.Level == 4).ToList(),
            LocationLevel5Stats = visitedLocationStats.Where(x => x.Level == 5).ToList(),
            FormsHistogram = formSubmissionsHistogram.Select(x => new HistogramPoint
            {
                Bucket = x.Bucket,
                Value = x.FormsSubmitted
            }).ToArray(),
            QuestionsHistogram = formSubmissionsHistogram.Select(x => new HistogramPoint
            {
                Bucket = x.Bucket,
                Value = x.NumberOfQuestionsAnswered
            }).ToArray(),
            FlaggedAnswersHistogram = formSubmissionsHistogram.Select(x => new HistogramPoint
            {
                Bucket = x.Bucket,
                Value = x.NumberOfFlaggedAnswers
            }).ToArray(),
            QuickReportsHistogram = quickReportsHistogram.ToArray(),
            IncidentReportsHistogram = incidentReportsHistogram.ToArray(),
            CitizenReportsHistogram = citizenReportsHistogram.ToArray()
        };
    }
}
