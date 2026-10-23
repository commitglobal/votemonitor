using Authorization.Policies;
using Authorization.Policies.Requirements;
using Dapper;
using Feature.Forms.Models;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Forms.ListV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<List<FormSlimModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/forms:listAll");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Lists all forms for an election round";
            s.Description = "Gets all forms without pagination";
        });
    }

    public override async Task<Results<Ok<List<FormSlimModel>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT *
                  FROM "GetNgoFormsList"(@electionRoundId, @ngoId, @searchText, @type, @status);
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            status = req.FormStatusFilter?.ToString(),
            type = req.TypeFilter?.ToString()
        };

        List<FormSlimModel> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            entries = (await dbConnection.QueryAsync<FormSlimModel>(sql, queryArgs)).ToList();
        }

        return TypedResults.Ok(entries);
    }
}
