using Feature.Review.Rules;
using Feature.Severity.Models;
using Vote.Monitor.Core.RulesEngine;

namespace Feature.Severity.Rules.Create;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<Created<SeverityRuleModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/severity-rules");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("automation-rules"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Adds a severity rule to the caller's NGO";
            s.Description = "The rule acts on records submitted or edited from now on; existing records don't change.";
        });
    }

    public override async Task<Results<Created<SeverityRuleModel>, NotFound>> ExecuteAsync(Request req,
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
            context.SeverityRules.Where(x => x.MonitoringNgoId == monitoringNgoId), req, null, ct));
        ThrowIfAnyErrors();

        var rule = SeverityRule.Create(req.ElectionRoundId,
            monitoringNgoId.Value,
            req.Name.Trim(),
            req.RecordTypes,
            FilterRuleJson.Serialize(req.Conditions),
            req.Severity);
        context.SeverityRules.Add(rule);
        await context.SaveChangesAsync(ct);

        var userNames = await context.GetUpdaterNamesAsync([rule], ct);
        return TypedResults.Created($"/api/election-rounds/{req.ElectionRoundId}/severity-rules/{rule.Id}",
            SeverityRuleModel.FromEntity(rule, userNames));
    }
}
