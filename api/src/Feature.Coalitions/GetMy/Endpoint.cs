using Authorization.Policies.Requirements;
using Feature.NgoCoalitions.Models;
using Microsoft.AspNetCore.Authorization;

namespace Feature.NgoCoalitions.GetMy;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<CoalitionModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/coalitions:my");
        DontAutoTag();
        Options(x => x.WithTags("coalitions"));
        Summary(s =>
        {
            s.Summary = "Gets coalition details for current ngo and selected election round";
        });
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<CoalitionModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var result =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!result.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var coalition = await context.Coalitions
            .Include(x => x.Memberships)
            .ThenInclude(x => x.MonitoringNgo)
            .ThenInclude(x => x.Ngo)
            .Include(x => x.Leader)
            .ThenInclude(x => x.Ngo)
            .Where(x => x.Memberships.Any(m => m.ElectionRoundId == req.ElectionRoundId
                                               && m.MonitoringNgo.NgoId == req.NgoId
                                               && m.MonitoringNgo.ElectionRoundId == req.ElectionRoundId))
            .Select(c => CoalitionModel.FromEntity(c))
            .FirstOrDefaultAsync(ct);

        if (coalition is null)
        {
            return TypedResults.Ok(new CoalitionModel { IsInCoalition = false });
        }

        return TypedResults.Ok(coalition);
    }
}
