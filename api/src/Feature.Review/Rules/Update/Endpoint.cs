using Feature.Review.Models;
using Vote.Monitor.Core.RulesEngine;

namespace Feature.Review.Rules.Update;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<ReviewerRuleModel>, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/reviewer-rules/{ruleId}");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("automation-rules"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Updates one of the caller's NGO's reviewer rules";
            s.Description = "The changes apply from now on; existing records don't change.";
        });
    }

    public override async Task<Results<Ok<ReviewerRuleModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var monitoringNgoId = await context.GetMonitoringNgoIdAsync(req.ElectionRoundId, req.NgoId, ct);
        var rule = await context.ReviewerRules
            .FirstOrDefaultAsync(x => x.Id == req.RuleId && x.MonitoringNgoId == monitoringNgoId, ct);
        if (rule is null)
        {
            return TypedResults.NotFound();
        }

        ValidationFailures.AddRange(await context.CheckReviewerRuleAsync(req, rule.MonitoringNgoId, rule, ct));
        ThrowIfAnyErrors();

        rule.Update(req.Name.Trim(),
            req.RecordTypes,
            FilterRuleJson.Serialize(req.Conditions),
            req.StageIds,
            req.ReviewerIds);
        await context.SaveChangesAsync(ct);

        var userNames = await context.GetUpdaterNamesAsync([rule], ct);
        return TypedResults.Ok(ReviewerRuleModel.FromEntity(rule, userNames));
    }
}
