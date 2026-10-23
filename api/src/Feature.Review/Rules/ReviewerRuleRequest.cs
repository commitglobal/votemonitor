using FluentValidation.Results;
using Feature.Review.Services;

namespace Feature.Review.Rules;

/// <summary>A reviewer rule, as sent when creating or updating one.</summary>
public class ReviewerRuleRequest : SaveRuleRequest
{
    /// <summary>"When a record enters" one of these stages.</summary>
    public Guid[] StageIds { get; set; } = [];

    /// <summary>The users to add as reviewers.</summary>
    public Guid[] ReviewerIds { get; set; } = [];
}

public abstract class ReviewerRuleRequestValidator<T> : SaveRuleRequestValidator<T> where T : ReviewerRuleRequest
{
    protected ReviewerRuleRequestValidator() : base(AutomationRuleKind.Reviewer)
    {
        RuleFor(x => x.StageIds)
            .NotEmpty()
            .Must(x => x.Distinct().Count() == x.Length)
            .WithMessage("Stages must be distinct");
        RuleFor(x => x.ReviewerIds)
            .NotEmpty()
            .Must(x => x.Length <= ReviewService.MaxReviewers)
            .WithMessage($"A rule can add at most {ReviewService.MaxReviewers} reviewers")
            .Must(x => x.Distinct().Count() == x.Length)
            .WithMessage("Reviewers must be distinct");
    }
}

public static class ReviewerRuleChecks
{
    /// <summary>
    /// What a reviewer rule can't be saved with, beyond its request validator: the shared checks, stages that aren't
    /// the NGO's, or new reviewers who aren't active admins or staff of the NGO. Reviewers already on the rule may stay
    /// after they leave or are deactivated; the rule skips them when it runs.
    /// </summary>
    /// <param name="rule">The rule being updated; null when creating one.</param>
    public static async Task<List<ValidationFailure>> CheckReviewerRuleAsync(this VoteMonitorContext context,
        ReviewerRuleRequest req,
        Guid monitoringNgoId,
        ReviewerRule? rule,
        CancellationToken ct)
    {
        var failures = await context.CheckRuleAsync(
            context.ReviewerRules.Where(x => x.MonitoringNgoId == monitoringNgoId), req, rule?.Id, ct);

        var stageCount = await context.ReviewStages
            .CountAsync(x => x.MonitoringNgoId == monitoringNgoId && req.StageIds.Contains(x.Id), ct);
        if (stageCount != req.StageIds.Length)
        {
            failures.Add(new ValidationFailure(nameof(req.StageIds),
                "Every stage must be one of your NGO's review stages in this election round"));
        }

        var newReviewerIds = req.ReviewerIds.Except(rule?.ReviewerIds ?? []).ToList();
        var assignable = await context.GetAssignableUsersAsync(req.NgoId, newReviewerIds, ct);
        if (assignable.Count != newReviewerIds.Count)
        {
            failures.Add(new ValidationFailure(nameof(req.ReviewerIds),
                "Every new reviewer must be an active admin or staff member of your NGO"));
        }

        return failures;
    }
}
