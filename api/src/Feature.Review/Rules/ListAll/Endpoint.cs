using Feature.Review.Models;

namespace Feature.Review.Rules.ListAll;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<ReviewerRuleModel[]>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/reviewer-rules");
        DontAutoTag();
        Options(x => x.WithTags("automation-rules"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Lists the caller's NGO's reviewer rules in the round, by name"; });
    }

    public override async Task<Results<Ok<ReviewerRuleModel[]>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var monitoringNgoId = await context.GetMonitoringNgoIdAsync(req.ElectionRoundId, req.NgoId, ct);
        var rules = await context.ReviewerRules
            .AsNoTracking()
            .Where(x => x.MonitoringNgoId == monitoringNgoId)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
        var userNames = await context.GetUpdaterNamesAsync(rules, ct);

        return TypedResults.Ok(rules.Select(x => ReviewerRuleModel.FromEntity(x, userNames)).ToArray());
    }
}
