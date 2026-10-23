using Feature.Review.Rules;
using Feature.Severity.Models;

namespace Feature.Severity.Rules.Get;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<SeverityRuleModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/severity-rules/{ruleId}");
        DontAutoTag();
        Options(x => x.WithTags("automation-rules"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Gets one of the caller's NGO's severity rules"; });
    }

    public override async Task<Results<Ok<SeverityRuleModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var monitoringNgoId = await context.GetMonitoringNgoIdAsync(req.ElectionRoundId, req.NgoId, ct);
        var rule = await context.SeverityRules
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == req.RuleId && x.MonitoringNgoId == monitoringNgoId, ct);
        if (rule is null)
        {
            return TypedResults.NotFound();
        }

        var userNames = await context.GetUpdaterNamesAsync([rule], ct);
        return TypedResults.Ok(SeverityRuleModel.FromEntity(rule, userNames));
    }
}
