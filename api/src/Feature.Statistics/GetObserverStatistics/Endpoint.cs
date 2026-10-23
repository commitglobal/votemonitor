using Authorization.Policies;
using Dapper;
using Feature.Statistics.Options;
using Microsoft.Extensions.Options;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;
using Vote.Monitor.Core.Models;

namespace Feature.Statistics.GetObserverStatistics;

public class Endpoint(
    INpgsqlConnectionFactory dbConnectionFactory,
    IFusionCache cache,
    IOptions<StatisticsFeatureOptions> options) : Endpoint<Request, Response>
{
    private readonly StatisticsFeatureOptions _options = options.Value;

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/statistics:my");
        DontAutoTag();
        Options(x => x.WithTags("mobile","statistics"));
        Summary(s => { s.Summary = "Observer statistics on election round"; });
        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<Response> ExecuteAsync(Request req, CancellationToken ct)
    {
        var cacheKey = $"statistics-{req.ElectionRoundId}-{req.ObserverId}";

        var cacheDuration = TimeSpan.FromMinutes(_options.CacheDurationInMinutes);

        return await cache.GetOrSetAsync(
            cacheKey,
            CachedResponse.Track(async _ => await GetObserverStatisticsAsync(req, ct), cacheDuration),
            cacheOptions => cacheOptions.SetDuration(cacheDuration),
            tags: [CacheTags.Statistics, CacheTags.ElectionRound(req.ElectionRoundId), CacheTags.Observer(req.ObserverId)],
            token: ct);
    }

    private async Task<Response> GetObserverStatisticsAsync(Request req, CancellationToken ct)
    {
        string sql = 
            """
            SELECT * FROM "GetObserverStatisticsTotals"(@electionRoundId, @observerId);
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            observerId = req.ObserverId,
        };
        
        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        return await dbConnection.QueryFirstAsync<Response>(sql, queryArgs);
    }
}
