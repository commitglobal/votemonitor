using Feature.Review.Rules;
using Feature.Review.Services;
using Microsoft.Extensions.Logging;

namespace Feature.Severity.Rules;

/// <summary>
/// Runs severity rules (this feature) and reviewer rules (<see cref="Feature.Review"/>) on records observers and
/// citizens save, and sends records their authors change back to the default stage; it lives here because this
/// feature sees both.
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
        RunAsync(electionRoundId, recordType, recordId, RecordChange.Created, ct);

    public Task RecordEditedAsync(Guid electionRoundId,
        ReportRecordType recordType,
        Guid recordId,
        CancellationToken ct) =>
        RunAsync(electionRoundId, recordType, recordId, RecordChange.Edited, ct);

    public Task RecordChangedByAuthorAsync(Guid electionRoundId,
        ReportRecordType recordType,
        Guid recordId,
        CancellationToken ct) =>
        RunAsync(electionRoundId, recordType, recordId, RecordChange.ChangedByAuthor, ct);

    public async Task FormSubmissionPartChangedByAuthorAsync(Guid electionRoundId,
        Guid submissionId,
        Guid pollingStationId,
        Guid formId,
        Guid monitoringObserverId,
        CancellationToken ct)
    {
        List<Guid> submissionIds;
        try
        {
            submissionIds = submissionId != Guid.Empty
                ? [submissionId]
                : await context.FormSubmissions
                    .Where(x => x.ElectionRoundId == electionRoundId
                                && x.PollingStationId == pollingStationId
                                && x.FormId == formId
                                && x.MonitoringObserverId == monitoringObserverId)
                    .Select(x => x.Id)
                    .ToListAsync(ct);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Could not find the form submissions of a note or attachment");
            return;
        }

        foreach (var id in submissionIds)
        {
            await RecordChangedByAuthorAsync(electionRoundId, ReportRecordType.FormSubmission, id, ct);
        }
    }

    private enum RecordChange
    {
        Created,
        Edited,
        ChangedByAuthor
    }

    private async Task RunAsync(Guid electionRoundId,
        ReportRecordType recordType,
        Guid recordId,
        RecordChange change,
        CancellationToken ct)
    {
        var isNew = change == RecordChange.Created;
        var returnsToDefaultStage = change == RecordChange.ChangedByAuthor;
        try
        {
            // Only the NGOs with something to do: rules that could run, or a stage to send the record back from. Each
            // sees the record (or not) through its own data source.
            var ngoIds = await context.MonitoringNgos
                .Where(x => x.ElectionRoundId == electionRoundId
                            && (context.SeverityRules.Any(rule => rule.MonitoringNgoId == x.Id)
                                || (isNew && context.ReviewerRules.Any(rule => rule.MonitoringNgoId == x.Id))
                                || (returnsToDefaultStage && context.ReviewWorkflows.Any(workflow =>
                                    workflow.MonitoringNgoId == x.Id
                                    && workflow.RecordType == recordType
                                    && workflow.RecordId == recordId
                                    && !workflow.Stage.IsDefault))))
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

                if (returnsToDefaultStage)
                {
                    await reviewService.ReturnToDefaultStageAsync(report, ct);
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
