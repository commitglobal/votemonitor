using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.ConnectionFactory;
using ZiggyCreatures.Caching.Fusion;

namespace Feature.Statistics.GetFormStatistics;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFusionCache cache) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/statistics/forms/{formId}");
        DontAutoTag();
        Options(x => x.WithTags("statistics"));
        Summary(s => { s.Summary = "Statistics for a specific form"; });
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var cacheKey = $"statistics-form-{req.ElectionRoundId}-{req.NgoId}-{req.FormId}-{req.DataSource}";

        var response = await cache.GetOrSetAsync(
            cacheKey,
            async _ => await GetStatisticsAsync(req, ct),
            options => options.SetDuration(StatisticsInstaller.DefaultCacheDuration),
            token: ct);

        return TypedResults.Ok(response);
    }

    private async Task<Response> GetStatisticsAsync(Request req, CancellationToken ct)
    {
        const string sql =
            """
            SELECT
                COUNT(*) AS "NumberOfSubmissions",
                COALESCE(SUM(FS."NumberOfQuestionsAnswered"), 0) AS "NumberOfQuestionsAnswered",
                COALESCE(SUM(FS."NumberOfFlaggedAnswers"), 0) AS "NumberOfFlaggedAnswers",
                COUNT(DISTINCT FS."MonitoringObserverId") AS "NumberOfObservers",
                COUNT(DISTINCT FS."PollingStationId") AS "NumberOfPollingStations"
            FROM "FormSubmissions" FS
            INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) AMO
                ON AMO."MonitoringObserverId" = FS."MonitoringObserverId"
            WHERE FS."ElectionRoundId" = @electionRoundId
              AND FS."FormId" = @formId;
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
