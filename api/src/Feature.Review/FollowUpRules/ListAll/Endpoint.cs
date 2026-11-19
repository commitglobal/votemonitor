using Feature.Review.Rules;
using Feature.Review.Models;

namespace Feature.Review.FollowUpRules.ListAll;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<FollowUpRuleModel[]>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/follow-up-rules");
        DontAutoTag();
        Options(x => x.WithTags("automation-rules"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Lists the caller's NGO's follow-up rules in the round, by name"; });
    }

    public override async Task<Results<Ok<FollowUpRuleModel[]>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var monitoringNgoId = await context.GetMonitoringNgoIdAsync(req.ElectionRoundId, req.NgoId, ct);
        var rules = await context.FollowUpRules
            .AsNoTracking()
            .Where(x => x.MonitoringNgoId == monitoringNgoId)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
        var userNames = await context.GetUpdaterNamesAsync(rules, ct);

        return TypedResults.Ok(rules.Select(x => FollowUpRuleModel.FromEntity(x, userNames)).ToArray());
    }
}
