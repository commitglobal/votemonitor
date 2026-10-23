using Feature.Review.Rules;
using Feature.Review.Services;
using Microsoft.Extensions.Logging;

namespace Feature.Severity.Rules;

/// <summary>
/// Runs severity rules (this feature) and reviewer rules (<see cref="Feature.Review"/>) on records observers and
/// citizens save; it lives here because this feature sees both.
/// </summary>
internal class RecordAutomation(
    VoteMonitorContext context,
    IReviewService reviewService,
    ISeverityRuleRunner severityRules,
    IReviewerRuleRunner reviewerRules,
    ILogger<RecordAutomation> logger) : IRecordAutomation
{
    public Task RecordCreatedAsync(Guid electionRoundId,
        ReportRecordType recordType,
        Guid recordId,
        CancellationToken ct) =>
        RunAsync(electionRoundId, recordType, recordId, isNew: true, ct);

    public Task RecordEditedAsync(Guid electionRoundId,
        ReportRecordType recordType,
        Guid recordId,
        CancellationToken ct) =>
        RunAsync(electionRoundId, recordType, recordId, isNew: false, ct);

    private async Task RunAsync(Guid electionRoundId,
        ReportRecordType recordType,
        Guid recordId,
        bool isNew,
        CancellationToken ct)
    {
        try
        {
            // Only the NGOs with rules that could run; each sees the record (or not) through its own data source.
            var ngoIds = await context.MonitoringNgos
                .Where(x => x.ElectionRoundId == electionRoundId
                            && (context.SeverityRules.Any(rule => rule.MonitoringNgoId == x.Id)
                                || (isNew && context.ReviewerRules.Any(rule => rule.MonitoringNgoId == x.Id))))
                .Select(x => x.NgoId)
                .ToListAsync(ct);

            foreach (var ngoId in ngoIds)
            {
                var report = await reviewService.ResolveAsync(electionRoundId, ngoId, recordType, recordId, ct);
                if (report is null)
                {
                    continue;
                }

                await severityRules.RunAsync(report, ct);
                if (isNew)
                {
                    // A new record has no stage of its own yet: it enters the NGO's default stage.
                    await reviewerRules.RunAsync(report, report.Stage, ct);
                }
            }
        }
        catch (Exception e)
        {
            // The record is saved; a failing rule must not fail the submission.
            context.ChangeTracker.Clear();
            logger.LogError(e, "Automation rules failed for {RecordType} {RecordId}", recordType, recordId);
        }
    }
}
