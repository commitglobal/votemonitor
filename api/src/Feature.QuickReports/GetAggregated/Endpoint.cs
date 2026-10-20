using Authorization.Policies;
using Authorization.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.QuickReports.GetAggregated;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/quick-reports:aggregated");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports"));
        Summary(s =>
        {
            s.Summary = "Gets the quick reports aggregated by incident category, location type and follow-up status, overall and per Level1..Level5 location";
        });
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

        var aggregate = await QuickReportsAggregateQuery.ExecuteAsync<QuickReportsAggregate>(dbConnectionFactory,
            req.ElectionRoundId, req.NgoId, req.DataSource, req.Filter, LocationLevel.Level1, LocationLevel.Level5, ct);

        return TypedResults.Ok(new Response { QuickReportsAggregate = aggregate });
    }
}
