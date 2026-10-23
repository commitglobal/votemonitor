using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Feature.Forms.Models;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Forms.Get;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, Results<Ok<FormFullModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/forms/{id}");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<Ok<FormFullModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT *
                  FROM "GetNgoFormDetails"(@electionRoundId, @ngoId, @id);
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            id = req.Id
        };

        FormFullModel? form;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            form = await dbConnection.QuerySingleOrDefaultAsync<FormFullModel>(sql, queryArgs);
        }

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(form);
    }
}
