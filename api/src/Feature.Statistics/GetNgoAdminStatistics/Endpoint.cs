using Microsoft.AspNetCore.Authorization;
using Authorization.Policies.Requirements;
using Authorization.Policies;
using Dapper;
using Feature.Statistics.GetNgoAdminStatistics.Models;
using Feature.Statistics.Options;
using Microsoft.Extensions.Options;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;
using Vote.Monitor.Core.Models;

namespace Feature.Statistics.GetNgoAdminStatistics;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFusionCache cache,
    IOptions<StatisticsFeatureOptions> options) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    private readonly StatisticsFeatureOptions _options = options.Value;

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/statistics");
        DontAutoTag();
        Options(x => x.WithTags("statistics"));
        Summary(s => { s.Summary = "Statistics for an election round"; });
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

        var cacheKey = $"statistics-{req.ElectionRoundId}-{req.NgoId}-{req.DataSource}";

        var cacheDuration = TimeSpan.FromMinutes(_options.CacheDurationInMinutes);

        var statistics = await cache.GetOrSetAsync(
            cacheKey,
            CachedResponse.Track(async _ => await GetNgoStatistics(req, ct), cacheDuration),
            cacheOptions => cacheOptions.SetDuration(cacheDuration),
            tags: [CacheTags.Statistics, CacheTags.ElectionRound(req.ElectionRoundId), CacheTags.Ngo(req.NgoId)],
            token: ct);

        return TypedResults.Ok(statistics);
    }

    private async Task<Response> GetNgoStatistics(Request req, CancellationToken ct)
    {
        string sql = 
            """
            SELECT * FROM "GetNgoAdminStatisticsObservers"(@electionRoundId, @ngoId, @dataSource);
            SELECT * FROM "GetNgoAdminStatisticsLevels"(@electionRoundId, @ngoId, @dataSource);
            SELECT * FROM "GetNgoAdminStatisticsFormsHistogram"(@electionRoundId, @ngoId, @dataSource);
            SELECT * FROM "GetNgoAdminStatisticsQuickReportsHistogram"(@electionRoundId, @ngoId, @dataSource);
            SELECT * FROM "GetNgoAdminStatisticsCitizenReportsHistogram"(@electionRoundId, @ngoId);
            SELECT * FROM "GetNgoAdminStatisticsIncidentReportsHistogram"(@electionRoundId, @ngoId, @dataSource);
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            dataSource = req.DataSource.ToString()
        };

        ObserversStats observersStats;
        List<VisitedPollingStationLevelStats> visitedPollingStationsStats;
        List<FormSubmissionsHistogramPoint> formSubmissionsHistogram;
        List<HistogramPoint> quickReportsHistogram;
        List<HistogramPoint> incidentReportsHistogram;
        List<HistogramPoint> citizenReportsHistogram;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using (var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs))
            {
                observersStats = multi.ReadSingle<ObserversStats>();
                visitedPollingStationsStats = multi.Read<VisitedPollingStationLevelStats>().ToList();
                formSubmissionsHistogram = multi.Read<FormSubmissionsHistogramPoint>().ToList();
                quickReportsHistogram = multi.Read<HistogramPoint>().ToList();
                incidentReportsHistogram = multi.Read<HistogramPoint>().ToList();
                citizenReportsHistogram = multi.Read<HistogramPoint>().ToList();
            }
        }

        return new Response
        {
            ObserversStats = observersStats,
            TotalStats = visitedPollingStationsStats.FirstOrDefault(x => x.Level == 0),
            Level1Stats = visitedPollingStationsStats.Where(x => x.Level == 1).ToList(),
            Level2Stats = visitedPollingStationsStats.Where(x => x.Level == 2).ToList(),
            Level3Stats = visitedPollingStationsStats.Where(x => x.Level == 3).ToList(),
            Level4Stats = visitedPollingStationsStats.Where(x => x.Level == 4).ToList(),
            Level5Stats = visitedPollingStationsStats.Where(x => x.Level == 5).ToList(),
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
