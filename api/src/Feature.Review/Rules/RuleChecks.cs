using FluentValidation.Results;

namespace Feature.Review.Rules;

public static class RuleChecks
{
    /// <summary>
    /// What a rule can't be saved with, beyond its request validator: a name another of the NGO's rules of this kind
    /// already has (ignoring case), or citizen reports in a round where the NGO doesn't do citizen reporting.
    /// </summary>
    /// <param name="rules">The NGO's rules of the kind being saved.</param>
    /// <param name="ruleId">The rule being updated; null when creating one.</param>
    public static async Task<List<ValidationFailure>> CheckRuleAsync(this VoteMonitorContext context,
        IQueryable<AutomationRule> rules,
        SaveRuleRequest req,
        Guid? ruleId,
        CancellationToken ct)
    {
        var failures = new List<ValidationFailure>();

        var name = req.Name.Trim().ToLower();
        if (await rules.AnyAsync(x => x.Id != ruleId && x.Name.ToLower() == name, ct))
        {
            failures.Add(new ValidationFailure(nameof(req.Name), "A rule with this name already exists"));
        }

        var isCitizenReportingNgo = await context.ElectionRounds.AnyAsync(x => x.Id == req.ElectionRoundId
            && x.CitizenReportingEnabled
            && x.MonitoringNgoForCitizenReporting!.NgoId == req.NgoId, ct);
        if (req.RecordTypes.Contains(ReportRecordType.CitizenReport) && !isCitizenReportingNgo)
        {
            failures.Add(new ValidationFailure(nameof(req.RecordTypes),
                "Citizen reports can only be used in rounds where your NGO does citizen reporting"));
        }

        return failures;
    }

    /// <summary>The display names of the users who last saved the rules.</summary>
    public static Task<Dictionary<Guid, string>> GetUpdaterNamesAsync(this VoteMonitorContext context,
        IEnumerable<AutomationRule> rules,
        CancellationToken ct)
    {
        var userIds = rules.Select(x => x.LastModifiedBy).Distinct().ToList();
        return context.Users
            .Where(x => userIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.DisplayName, ct);
    }
}
