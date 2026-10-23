using Feature.Review.Models;
using Vote.Monitor.Core.RulesEngine;

namespace Feature.Review.Rules.Create;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<Created<ReviewerRuleModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/reviewer-rules");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("automation-rules"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Adds a reviewer rule to the caller's NGO";
            s.Description = "The rule acts on records entering its stages from now on; existing records don't change.";
        });
    }

    public override async Task<Results<Created<ReviewerRuleModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var monitoringNgoId = await context.GetMonitoringNgoIdAsync(req.ElectionRoundId, req.NgoId, ct);
        if (monitoringNgoId is null)
        {
            return TypedResults.NotFound();
        }

        ValidationFailures.AddRange(await context.CheckReviewerRuleAsync(req, monitoringNgoId.Value, null, ct));
        ThrowIfAnyErrors();

        var rule = ReviewerRule.Create(req.ElectionRoundId,
            monitoringNgoId.Value,
            req.Name.Trim(),
            req.RecordTypes,
            FilterRuleJson.Serialize(req.Conditions),
            req.StageIds,
            req.ReviewerIds);
        context.ReviewerRules.Add(rule);
        await context.SaveChangesAsync(ct);

        var userNames = await context.GetUpdaterNamesAsync([rule], ct);
        return TypedResults.Created($"/api/election-rounds/{req.ElectionRoundId}/reviewer-rules/{rule.Id}",
            ReviewerRuleModel.FromEntity(rule, userNames));
    }
}
