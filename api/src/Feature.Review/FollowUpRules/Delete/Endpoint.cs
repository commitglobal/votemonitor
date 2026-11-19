namespace Feature.Review.FollowUpRules.Delete;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/follow-up-rules/{ruleId}");
        DontAutoTag();
        Options(x => x.WithTags("automation-rules"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Deletes one of the caller's NGO's follow-up rules";
            s.Description = "Follow-up statuses the rule set stay; its history entries keep its name.";
        });
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var monitoringNgoId = await context.GetMonitoringNgoIdAsync(req.ElectionRoundId, req.NgoId, ct);
        var deleted = await context.FollowUpRules
            .Where(x => x.Id == req.RuleId && x.MonitoringNgoId == monitoringNgoId)
            .ExecuteDeleteAsync(ct);

        return deleted == 0 ? TypedResults.NotFound() : TypedResults.NoContent();
    }
}
