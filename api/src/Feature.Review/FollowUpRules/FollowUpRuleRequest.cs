using Feature.Review.Rules;
using FluentValidation.Results;

namespace Feature.Review.FollowUpRules;

/// <summary>A follow-up rule, as sent when creating or updating one.</summary>
public class FollowUpRuleRequest : SaveRuleRequest
{
    /// <summary>"Set follow-up to" this one of the NGO's follow-up statuses.</summary>
    public Guid FollowUpStatusId { get; set; }
}

public abstract class FollowUpRuleRequestValidator<T> : SaveRuleRequestValidator<T> where T : FollowUpRuleRequest
{
    protected FollowUpRuleRequestValidator() : base(AutomationRuleKind.FollowUp)
    {
        RuleFor(x => x.FollowUpStatusId).NotEmpty();
    }
}

public static class FollowUpRuleChecks
{
    /// <summary>
    /// What a follow-up rule can't be saved with, beyond its request validator: the shared checks, or a status that
    /// isn't one of the NGO's follow-up statuses in the round. The default is allowed (it never changes anything).
    /// </summary>
    /// <param name="ruleId">The rule being updated; null when creating one.</param>
    public static async Task<List<ValidationFailure>> CheckFollowUpRuleAsync(this VoteMonitorContext context,
        FollowUpRuleRequest req,
        Guid monitoringNgoId,
        Guid? ruleId,
        CancellationToken ct)
    {
        var failures = await context.CheckRuleAsync(
            context.FollowUpRules.Where(x => x.MonitoringNgoId == monitoringNgoId), req, ruleId, ct);

        var isOwnStatus = await context.FollowUpStatuses
            .AnyAsync(x => x.Id == req.FollowUpStatusId && x.MonitoringNgoId == monitoringNgoId, ct);
        if (!isOwnStatus)
        {
            failures.Add(new ValidationFailure(nameof(req.FollowUpStatusId),
                "Pick one of your NGO's follow-up statuses in this election round"));
        }

        return failures;
    }
}
