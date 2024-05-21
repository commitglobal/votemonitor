using System.Data;
using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Microsoft.AspNetCore.Authorization;

namespace Feature.MonitoringObservers.Get;

public class Endpoint(IAuthorizationService authorizationService, IDbConnection dbConnection) : Endpoint<Request, Results<Ok<MonitoringObserverModel>, NotFound>>
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
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<MonitoringObserverModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = @"
            SELECT
                MO.""Id"",
                U.""FirstName"",
                U.""LastName"",
                U.""PhoneNumber"",
                U.""Email"",
                MO.""Tags"",
                MO.""Status""
            FROM
                ""MonitoringObservers"" MO
                INNER JOIN ""MonitoringNgos"" MN ON MN.""Id"" = MO.""MonitoringNgoId""
                INNER JOIN ""Observers"" O ON O.""Id"" = MO.""ObserverId""
                INNER JOIN ""AspNetUsers"" U ON U.""Id"" = O.""ApplicationUserId""
            WHERE
                MN.""ElectionRoundId"" = @electionRoundId
                AND MN.""NgoId"" = @ngoId
                AND MO.""Id"" = @id";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            id = req.Id
        };

        var monitoringObserver = await dbConnection.QuerySingleAsync<MonitoringObserverModel>(sql, queryArgs);
        if (monitoringObserver is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(monitoringObserver);
    }
}
