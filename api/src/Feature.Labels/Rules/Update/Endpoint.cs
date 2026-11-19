using Feature.Labels.Services;
using Feature.Review.Rules;
using Feature.Labels.Models;
using Vote.Monitor.Core.RulesEngine;

namespace Feature.Labels.Rules.Update;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<Ok<LabelRuleModel>, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/label-rules/{ruleId}");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("automation-rules"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Updates one of the caller's NGO's label rules";
            s.Description = "The changes apply from now on; existing records don't change.";
        });
    }

    public override async Task<Results<Ok<LabelRuleModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var monitoringNgoId = await context.GetMonitoringNgoIdAsync(req.ElectionRoundId, req.NgoId, ct);
        var rule = await context.LabelRules
            .FirstOrDefaultAsync(x => x.Id == req.RuleId && x.MonitoringNgoId == monitoringNgoId, ct);
        if (rule is null)
        {
            return TypedResults.NotFound();
        }

        ValidationFailures.AddRange(await context.CheckRuleAsync(
            context.LabelRules.Where(x => x.MonitoringNgoId == rule.MonitoringNgoId), req, rule.Id, ct));
        ThrowIfAnyErrors();

        rule.Update(req.Name.Trim(),
            req.RecordTypes,
            FilterRuleJson.Serialize(req.Conditions),
            LabelService.Normalize(req.Labels).ToArray());
        await context.SaveChangesAsync(ct);

        var userNames = await context.GetUpdaterNamesAsync([rule], ct);
        return TypedResults.Ok(LabelRuleModel.FromEntity(rule, userNames));
    }
}
