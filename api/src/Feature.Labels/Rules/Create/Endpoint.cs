using Feature.Labels.Services;
using Feature.Review.Rules;
using Feature.Labels.Models;
using Vote.Monitor.Core.RulesEngine;

namespace Feature.Labels.Rules.Create;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<Created<LabelRuleModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/label-rules");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("automation-rules"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Adds a label rule to the caller's NGO";
            s.Description = "The rule acts on records submitted or edited from now on; existing records don't change.";
        });
    }

    public override async Task<Results<Created<LabelRuleModel>, NotFound>> ExecuteAsync(Request req,
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

        ValidationFailures.AddRange(await context.CheckRuleAsync(
            context.LabelRules.Where(x => x.MonitoringNgoId == monitoringNgoId), req, null, ct));
        ThrowIfAnyErrors();

        var rule = LabelRule.Create(req.ElectionRoundId,
            monitoringNgoId.Value,
            req.Name.Trim(),
            req.RecordTypes,
            FilterRuleJson.Serialize(req.Conditions),
            LabelService.Normalize(req.Labels).ToArray());
        context.LabelRules.Add(rule);
        await context.SaveChangesAsync(ct);

        var userNames = await context.GetUpdaterNamesAsync([rule], ct);
        return TypedResults.Created($"/api/election-rounds/{req.ElectionRoundId}/label-rules/{rule.Id}",
            LabelRuleModel.FromEntity(rule, userNames));
    }
}
