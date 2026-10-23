using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.MonitoringObservers.Get;

public class Endpoint(IAuthorizationService authorizationService, INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, Results<Ok<MonitoringObserverModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/monitoring-observers/{id}");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("monitoring-observers"));
        Summary(s =>
        {
            s.Summary = "Gets monitoring observer details";
        });
        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<Ok<MonitoringObserverModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql =
        """
        SELECT *
        FROM "GetMonitoringObserverDetails"(@electionRoundId, @ngoId, @id);
        """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            id = req.Id
        };

        MonitoringObserverModel? monitoringObserver;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            monitoringObserver = await dbConnection.QuerySingleOrDefaultAsync<MonitoringObserverModel>(sql, queryArgs);
        }

        if (monitoringObserver is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(monitoringObserver);
    }
}
