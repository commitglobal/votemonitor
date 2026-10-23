using Authorization.Policies;
using Dapper;
using Feature.Statistics.GetNgoAdminStatistics.Models;
using Feature.Statistics.Options;
using Microsoft.Extensions.Options;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;
using Vote.Monitor.Core.Models;

namespace Feature.Statistics.GetMonitoringNgoStatistics;

public class Endpoint(
    INpgsqlConnectionFactory dbConnectionFactory,
    IFusionCache cache,
    IOptions<StatisticsFeatureOptions> options) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    private readonly StatisticsFeatureOptions _options = options.Value;

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/monitoring-ngos/{monitoringNgoId}:stats");
        DontAutoTag();
        Options(x => x.WithTags("statistics"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Statistics for a monitoring NGO in an election round";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var cacheKey = $"statistics-monitoring-ngo-{req.ElectionRoundId}-{req.MonitoringNgoId}";

        var cached = await cache.TryGetAsync<Response>(cacheKey, token: ct);
        if (cached.HasValue)
        {
            return TypedResults.Ok(cached.Value);
        }

        var result = await GetStatisticsAsync(req, ct);
        if (result is null)
        {
            return TypedResults.NotFound();
        }

        var cacheDuration = TimeSpan.FromMinutes(_options.CacheDurationInMinutes);
        result.MarkCached(cacheDuration);

        await cache.SetAsync(
            cacheKey,
            result,
            options => options.SetDuration(cacheDuration),
            tags: [CacheTags.Statistics, CacheTags.ElectionRound(req.ElectionRoundId), CacheTags.MonitoringNgo(req.MonitoringNgoId)],
            token: ct);

        return TypedResults.Ok(result);
    }

    private async Task<Response?> GetStatisticsAsync(Request req, CancellationToken ct)
    {
        const string sql =
            """
            SELECT * FROM "StatsOverviewMonitoringNgoStatus"(@electionRoundId, @monitoringNgoId);
            SELECT * FROM "StatsOverviewObservers"(@electionRoundId, @monitoringNgoId, NULL);
            SELECT * FROM "StatsOverviewPollingStationLevels"(@electionRoundId, @monitoringNgoId, NULL);
            SELECT * FROM "StatsOverviewLocationLevels"(@electionRoundId, @monitoringNgoId, NULL);
            SELECT * FROM "StatsOverviewFormsHistogram"(@electionRoundId, @monitoringNgoId, NULL) ORDER BY "Bucket";
            SELECT * FROM "StatsOverviewAnswersHistogram"(@electionRoundId, @monitoringNgoId, NULL) ORDER BY "Bucket";
            SELECT * FROM "StatsOverviewQuickReportsHistogram"(@electionRoundId, @monitoringNgoId, NULL);
            SELECT * FROM "StatsOverviewCitizenReportsHistogram"(@electionRoundId, @monitoringNgoId, NULL);
            SELECT * FROM "StatsOverviewIncidentReportsHistogram"(@electionRoundId, @monitoringNgoId, NULL);
            SELECT * FROM "StatsOverviewCitizenReportAnswersHistogram"(@electionRoundId, @monitoringNgoId, NULL) ORDER BY "Bucket";
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            monitoringNgoId = req.MonitoringNgoId
        };

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);

        var status = multi.Read<string>().SingleOrDefault();
        if (status is null)
        {
            return null;
        }

        var observersStats = multi.ReadSingle<ObserversStats>();
        var visitedPollingStationsStats = multi.Read<VisitedPollingStationLevelStats>().ToList();
        var visitedLocationStats = multi.Read<VisitedLocationLevelStats>().ToList();
        var formSubmissionsHistogram = multi.Read<FormSubmissionsHistogramPoint>().ToList();
        var answersHistogram = multi.Read<AnswersHistogramPoint>().ToList();
        var quickReportsHistogram = multi.Read<HistogramPoint>().ToList();
        var citizenReportsHistogram = multi.Read<HistogramPoint>().ToList();
        var incidentReportsHistogram = multi.Read<HistogramPoint>().ToList();
        var citizenReportsAnswersHistogram = multi.Read<CitizenReportsHistogramPoint>().ToList();

        return new Response
        {
            Status = status,
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
            AnswersPerHourHistogram = answersHistogram.Select(x => new HistogramPoint
            {
                Bucket = x.Bucket,
                Value = x.NumberOfAnswers
            }).ToArray(),
            FlaggedAnswersPerHourHistogram = answersHistogram.Select(x => new HistogramPoint
            {
                Bucket = x.Bucket,
                Value = x.NumberOfFlaggedAnswers
            }).ToArray(),
            QuickReportsHistogram = quickReportsHistogram.ToArray(),
            IncidentReportsHistogram = incidentReportsHistogram.ToArray(),
            CitizenReportsHistogram = citizenReportsHistogram.ToArray(),
            CitizenReportsSubmissionsPerHourHistogram = citizenReportsAnswersHistogram.Select(x => new HistogramPoint
            {
                Bucket = x.Bucket,
                Value = x.NumberOfSubmissions
            }).ToArray(),
            CitizenReportsAnswersPerHourHistogram = citizenReportsAnswersHistogram.Select(x => new HistogramPoint
            {
                Bucket = x.Bucket,
                Value = x.NumberOfAnswers
            }).ToArray(),
            CitizenReportsFlaggedAnswersPerHourHistogram = citizenReportsAnswersHistogram.Select(x => new HistogramPoint
            {
                Bucket = x.Bucket,
                Value = x.NumberOfFlaggedAnswers
            }).ToArray()
        };
    }
}
