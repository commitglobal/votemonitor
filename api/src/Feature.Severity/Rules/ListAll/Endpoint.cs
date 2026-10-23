using Feature.Review.Rules;
using Feature.Severity.Models;

namespace Feature.Severity.Rules.ListAll;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<SeverityRuleModel[]>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/severity-rules");
        DontAutoTag();
        Options(x => x.WithTags("automation-rules"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s => { s.Summary = "Lists the caller's NGO's severity rules in the round, by name"; });
    }

    public override async Task<Results<Ok<SeverityRuleModel[]>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var monitoringNgoId = await context.GetMonitoringNgoIdAsync(req.ElectionRoundId, req.NgoId, ct);
        var rules = await context.SeverityRules
            .AsNoTracking()
            .Where(x => x.MonitoringNgoId == monitoringNgoId)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
        var userNames = await context.GetUpdaterNamesAsync(rules, ct);

        return TypedResults.Ok(rules.Select(x => SeverityRuleModel.FromEntity(x, userNames)).ToArray());
    }
}
