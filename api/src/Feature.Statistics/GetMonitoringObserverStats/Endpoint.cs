using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Feature.Statistics.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;
using Vote.Monitor.Core.Models;

namespace Feature.Statistics.GetMonitoringObserverStats;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFusionCache cache,
    IOptions<StatisticsFeatureOptions> options) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    private readonly StatisticsFeatureOptions _options = options.Value;

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/monitoring-observers/{monitoringObserverId}:stats");
        DontAutoTag();
        Options(x => x.WithTags("statistics", "monitoring-observers"));
        Summary(s => { s.Summary = "Workload statistics for a specific monitoring observer"; });
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
            $"monitoring-observer-stats-{req.ElectionRoundId}-{req.NgoId}-{req.MonitoringObserverId}-{req.DataSource}";

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
            SELECT * FROM "GetMonitoringObserverStatsWorkload"(@electionRoundId, @ngoId, @monitoringObserverId, @dataSource);
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            monitoringObserverId = req.MonitoringObserverId,
            dataSource = req.DataSource.ToString()
        };

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        return await dbConnection.QueryFirstAsync<Response>(sql, queryArgs);
    }
}
