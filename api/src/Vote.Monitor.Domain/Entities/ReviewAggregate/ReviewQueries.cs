namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

public static class ReviewQueries
{
    /// <summary>The NGO's monitoring NGO in the round, if it monitors it.</summary>
    public static Task<Guid?> GetMonitoringNgoIdAsync(this VoteMonitorContext context,
        Guid electionRoundId,
        Guid ngoId,
        CancellationToken ct) =>
        context.MonitoringNgos
            .Where(x => x.ElectionRoundId == electionRoundId && x.NgoId == ngoId)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync(ct);

    /// <summary>
    /// The fixed follow-up value set through the older status endpoints (the record's own column): the caller's NGO
    /// also moves the record to its own status with that value (one of the three every NGO starts with), and the
    /// change goes in its report history under that status's name. The caller saves the context.
    /// </summary>
    public static async Task AddFollowUpStatusActivityAsync(this VoteMonitorContext context,
        Guid electionRoundId,
        Guid ngoId,
        ReportRecordType recordType,
        Guid recordId,
        Guid userId,
        string followUpStatus,
        DateTime now,
        CancellationToken ct)
    {
        var monitoringNgoId = await context.GetMonitoringNgoIdAsync(electionRoundId, ngoId, ct);
        var actorName = await context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.DisplayName)
            .FirstOrDefaultAsync(ct);
        if (monitoringNgoId is null || actorName is null)
        {
            return;
        }

        var status = await context.FollowUpStatuses.FirstOrDefaultAsync(
            x => x.MonitoringNgoId == monitoringNgoId && x.LegacyStatus == followUpStatus, ct);
        var name = followUpStatus;
        if (status is not null)
        {
            name = status.Name;
            var row = await context.RecordFollowUps.FirstOrDefaultAsync(x => x.MonitoringNgoId == monitoringNgoId
                                                                         && x.RecordType == recordType
                                                                         && x.RecordId == recordId, ct);
            if (row is null)
            {
                context.RecordFollowUps.Add(FollowUpAggregate.RecordFollowUp.Create(electionRoundId, monitoringNgoId.Value,
                    recordType, recordId, status, userId, now));
            }
            else
            {
                row.Change(status, userId, now);
            }
        }

        context.ReviewActivities.Add(ReviewActivity.FollowUpStatusChanged(
            new ReviewTarget(electionRoundId, monitoringNgoId.Value, recordType, recordId),
            new ReviewActor(userId, actorName),
            name,
            now));
    }

    /// <summary>The caller NGO's follow-up status for a record: its own row's, else its default; null outside the round.</summary>
    public static async Task<FollowUpAggregate.FollowUpStatusDefinition?> GetFollowUpStatusAsync(this VoteMonitorContext context,
        Guid electionRoundId,
        Guid ngoId,
        ReportRecordType recordType,
        Guid recordId,
        CancellationToken ct)
    {
        var monitoringNgoId = await context.GetMonitoringNgoIdAsync(electionRoundId, ngoId, ct);
        if (monitoringNgoId is null)
        {
            return null;
        }

        return await context.RecordFollowUps
                   .AsNoTracking()
                   .Where(x => x.MonitoringNgoId == monitoringNgoId && x.RecordType == recordType && x.RecordId == recordId)
                   .Select(x => x.FollowUpStatus)
                   .FirstOrDefaultAsync(ct)
               ?? await context.FollowUpStatuses.AsNoTracking()
                   .FirstOrDefaultAsync(x => x.MonitoringNgoId == monitoringNgoId && x.IsDefault, ct);
    }

    /// <summary>Removes every NGO's workflow, reviewers and history, and its answer history, for a deleted record.</summary>
    public static async Task DeleteReviewAsync(this VoteMonitorContext context,
        ReportRecordType recordType,
        Guid recordId,
        CancellationToken ct)
    {
        await context.ReviewWorkflows
            .Where(x => x.RecordType == recordType && x.RecordId == recordId)
            .ExecuteDeleteAsync(ct);
        await context.Reviewers
            .Where(x => x.RecordType == recordType && x.RecordId == recordId)
            .ExecuteDeleteAsync(ct);
        await context.ReviewActivities
            .Where(x => x.RecordType == recordType && x.RecordId == recordId)
            .ExecuteDeleteAsync(ct);
        await context.AnswerChanges
            .Where(x => x.RecordType == recordType && x.RecordId == recordId)
            .ExecuteDeleteAsync(ct);
        await context.RecordFollowUps
            .Where(x => x.RecordType == recordType && x.RecordId == recordId)
            .ExecuteDeleteAsync(ct);
    }
}
