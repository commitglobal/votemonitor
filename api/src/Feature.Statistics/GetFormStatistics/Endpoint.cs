using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Feature.Statistics.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;
using Vote.Monitor.Core.Models;

namespace Feature.Statistics.GetFormStatistics;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFusionCache cache,
    IOptions<StatisticsFeatureOptions> options) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    private readonly StatisticsFeatureOptions _options = options.Value;

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/statistics/forms/{formId}");
        DontAutoTag();
        Options(x => x.WithTags("statistics"));
        Summary(s => { s.Summary = "Statistics for a specific form"; });
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

        var cacheKey = $"statistics-form-{req.ElectionRoundId}-{req.NgoId}-{req.FormId}-{req.DataSource}";

        var cacheDuration = TimeSpan.FromMinutes(_options.CacheDurationInMinutes);

        var response = await cache.GetOrSetAsync(
            cacheKey,
            CachedResponse.Track(async _ => await GetStatisticsAsync(req, ct), cacheDuration),
            cacheOptions => cacheOptions.SetDuration(cacheDuration),
            tags: [CacheTags.Statistics, CacheTags.ElectionRound(req.ElectionRoundId), CacheTags.Ngo(req.NgoId), CacheTags.Form(req.FormId)],
            token: ct);

        return TypedResults.Ok(response);
    }

    private async Task<Response> GetStatisticsAsync(Request req, CancellationToken ct)
    {
        const string sql =
            """
            SELECT * FROM "GetFormStatisticsTotals"(@electionRoundId, @ngoId, @formId, @dataSource);
            """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            formId = req.FormId,
            dataSource = req.DataSource.ToString()
        };

        using var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct);
        return await dbConnection.QueryFirstAsync<Response>(sql, queryArgs);
    }
}
