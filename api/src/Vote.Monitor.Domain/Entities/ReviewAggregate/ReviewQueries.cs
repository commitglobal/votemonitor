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
    /// Records a follow-up status change in the caller's report history. The caller saves the context.
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

        context.ReviewActivities.Add(ReviewActivity.FollowUpStatusChanged(
            new ReviewTarget(electionRoundId, monitoringNgoId.Value, recordType, recordId),
            new ReviewActor(userId, actorName),
            followUpStatus,
            now));
    }

    /// <summary>Removes every NGO's workflow, reviewers and history for a deleted record.</summary>
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
    }
}
