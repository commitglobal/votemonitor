using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Feature.Statistics.GetNgoAdminStatistics.Models;
using Feature.Statistics.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;
using Vote.Monitor.Core.Models;

namespace Feature.Statistics.GetMonitoringObserverStatistics;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFusionCache cache,
    IOptions<StatisticsFeatureOptions> options) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    private readonly StatisticsFeatureOptions _options = options.Value;

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/statistics/observers/{monitoringObserverId}");
        DontAutoTag();
        Options(x => x.WithTags("statistics"));
        Summary(s => { s.Summary = "Statistics for a specific monitoring observer"; });
        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var cacheKey =
            $"statistics-observer-{req.ElectionRoundId}-{req.NgoId}-{req.MonitoringObserverId}-{req.DataSource}";

        var cacheDuration = TimeSpan.FromMinutes(_options.CacheDurationInMinutes);

        var response = await cache.GetOrSetAsync(
            cacheKey,
            CachedResponse.Track(async _ => await GetStatisticsAsync(req, ct), cacheDuration),
            cacheOptions => cacheOptions.SetDuration(cacheDuration),
            tags: [CacheTags.Statistics, CacheTags.ElectionRound(req.ElectionRoundId), CacheTags.Ngo(req.NgoId), CacheTags.MonitoringObserver(req.MonitoringObserverId)],
            token: ct);

        return TypedResults.Ok(response);
    }

    private async Task<Response> GetStatisticsAsync(Request req, CancellationToken ct)
    {
        const string sql =
            """
            SELECT * FROM "GetMonitoringObserverPollingStationLevelStats"(@electionRoundId, @ngoId, @monitoringObserverId, @dataSource);
            SELECT * FROM "GetMonitoringObserverStatisticsExtras"(@electionRoundId, @ngoId, @monitoringObserverId, @dataSource);
            SELECT * FROM "GetMonitoringObserverStatisticsAnswersHistogram"(@electionRoundId, @ngoId, @monitoringObserverId, @dataSource);
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            monitoringObserverId = req.MonitoringObserverId,
            dataSource = req.DataSource.ToString()
        };

        List<VisitedPollingStationLevelStats> levelStats;
        ObserverExtras extras;
        List<AnswersHistogramPoint> answersHistogram;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        using (var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs))
        {
            levelStats = multi.Read<VisitedPollingStationLevelStats>().ToList();
            extras = multi.ReadSingle<ObserverExtras>();
            answersHistogram = multi.Read<AnswersHistogramPoint>().ToList();
        }

        var totalStats = levelStats.FirstOrDefault(x => x.Level == 0);

        return new Response
        {
            TotalStats = totalStats,
            Level1Stats = levelStats.Where(x => x.Level == 1).ToList(),
            Level2Stats = levelStats.Where(x => x.Level == 2).ToList(),
            Level3Stats = levelStats.Where(x => x.Level == 3).ToList(),
            Level4Stats = levelStats.Where(x => x.Level == 4).ToList(),
            Level5Stats = levelStats.Where(x => x.Level == 5).ToList(),
            NumberOfFormsSubmitted = totalStats?.NumberOfFormSubmissions ?? 0,
            NumberOfQuestionsAnswered = totalStats?.NumberOfQuestionsAnswered ?? 0,
            NumberOfFlaggedAnswers = totalStats?.NumberOfFlaggedAnswers ?? 0,
            NumberOfQuickReports = extras.NumberOfQuickReports,
            NumberOfIncidentReports = extras.NumberOfIncidentReports,
            NumberOfPollingStationsVisited = totalStats?.NumberOfVisitedPollingStations ?? 0,
            MinutesMonitoring = totalStats?.MinutesMonitoring ?? 0,
            NumberOfNotes = extras.NumberOfNotes,
            NumberOfAttachments = extras.NumberOfAttachments,
            AnswersPerHourHistogram = answersHistogram.Select(x => new HistogramPoint
            {
                Bucket = x.Bucket,
                Value = x.NumberOfAnswers
            }).ToArray(),
            FlaggedAnswersPerHourHistogram = answersHistogram.Select(x => new HistogramPoint
            {
                Bucket = x.Bucket,
                Value = x.NumberOfFlaggedAnswers
            }).ToArray()
        };
    }

    private sealed class ObserverExtras
    {
        public int NumberOfNotes { get; set; }
        public int NumberOfAttachments { get; set; }
        public int NumberOfQuickReports { get; set; }
        public int NumberOfIncidentReports { get; set; }
    }
}
