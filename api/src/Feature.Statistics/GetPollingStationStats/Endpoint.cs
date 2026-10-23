using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Feature.Statistics.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;

namespace Feature.Statistics.GetPollingStationStats;

public class Endpoint(
    IAuthorizationService authorizationService,
    ICurrentUserRoleProvider userRoleProvider,
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
        Policies(PolicyNames.NotObservers);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var isPlatformAdmin = userRoleProvider.IsPlatformAdmin();

        if (!isPlatformAdmin)
        {
            var authorizationResult =
                await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
            if (!authorizationResult.Succeeded || !req.NgoId.HasValue || req.NgoId == Guid.Empty)
            {
                return TypedResults.NotFound();
            }
        }

        var dataSource = req.DataSource ?? DataSource.Ngo;

        var cacheKey = isPlatformAdmin
            ? $"polling-station-stats-{req.ElectionRoundId}-all-{req.PollingStationId}"
            : $"polling-station-stats-{req.ElectionRoundId}-{req.NgoId}-{req.PollingStationId}-{dataSource}";

        var cacheDuration = TimeSpan.FromMinutes(_options.CacheDurationInMinutes);

        List<string> cacheTags =
            [CacheTags.Statistics, CacheTags.ElectionRound(req.ElectionRoundId), CacheTags.PollingStation(req.PollingStationId)];
        if (!isPlatformAdmin)
        {
            cacheTags.Add(CacheTags.Ngo(req.NgoId!.Value));
        }

        var response = await cache.GetOrSetAsync(
            cacheKey,
            CachedResponse.Track(async _ => await GetStatisticsAsync(req, isPlatformAdmin, dataSource, ct), cacheDuration),
            cacheOptions => cacheOptions.SetDuration(cacheDuration),
            tags: cacheTags,
            token: ct);

        return TypedResults.Ok(response);
    }

    private async Task<Response> GetStatisticsAsync(Request req, bool isPlatformAdmin, DataSource dataSource,
        CancellationToken ct)
    {
        const string sql =
            """
            SELECT * FROM "GetPollingStationStatsOverview"(@electionRoundId, @ngoId, @pollingStationId, @dataSource, @isPlatformAdmin);
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            pollingStationId = req.PollingStationId,
            dataSource = dataSource.Value,
            isPlatformAdmin
        };

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        return await dbConnection.QueryFirstAsync<Response>(sql, queryArgs);
    }
}
