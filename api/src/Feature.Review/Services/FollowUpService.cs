using Npgsql;
using Vote.Monitor.Domain.Entities.FollowUpAggregate;

namespace Feature.Review.Services;

public interface IFollowUpService
{
    /// <summary>The caller NGO's follow-up status for the report: its own row's, else the NGO's default.</summary>
    Task<FollowUpStatusDefinition> GetCurrentAsync(ReviewTarget target, CancellationToken ct);

    /// <summary>
    /// Moves the report to another of the NGO's follow-up statuses. Setting the current one changes nothing; every
    /// real change is recorded in the report's history (with the status's name) and its reviewers are notified. When
    /// the NGO owns the report and the status is one of the three every NGO starts with, the report's own follow-up
    /// column (read by the mobile app and older endpoints) follows.
    /// </summary>
    Task SetAsync(ReviewContext report, FollowUpStatusDefinition status, ReviewActor actor, CancellationToken ct);

    /// <summary>
    /// Before another status becomes the default: gives every report still on the old default its own row, so
    /// switching the default doesn't move existing reports.
    /// </summary>
    Task MaterializeDefaultAsync(Guid electionRoundId, Guid ngoId, FollowUpStatusDefinition previousDefault, CancellationToken ct);
}

internal class FollowUpService(VoteMonitorContext context, IReviewService reviewService, ITimeProvider timeProvider)
    : IFollowUpService
{
    private Task<RecordFollowUp?> GetRowAsync(ReviewTarget target, CancellationToken ct) =>
        context.RecordFollowUps
            .Include(x => x.FollowUpStatus)
            .FirstOrDefaultAsync(x => x.MonitoringNgoId == target.MonitoringNgoId
                                      && x.RecordType == target.RecordType
                                      && x.RecordId == target.RecordId, ct);

    public async Task<FollowUpStatusDefinition> GetCurrentAsync(ReviewTarget target, CancellationToken ct) =>
        (await GetRowAsync(target, ct))?.FollowUpStatus
        ?? await context.FollowUpStatuses.FirstAsync(x => x.MonitoringNgoId == target.MonitoringNgoId && x.IsDefault, ct);

    public async Task SetAsync(ReviewContext report, FollowUpStatusDefinition status, ReviewActor actor, CancellationToken ct)
    {
        var now = timeProvider.UtcNow;
        var row = await GetRowAsync(report.Target, ct);
        var from = row?.FollowUpStatus
                   ?? await context.FollowUpStatuses.FirstAsync(x => x.MonitoringNgoId == report.Target.MonitoringNgoId && x.IsDefault, ct);
        if (from.Id == status.Id)
        {
            return;
        }

        if (row is null)
        {
            context.RecordFollowUps.Add(RecordFollowUp.Create(report.Target.ElectionRoundId,
                report.Target.MonitoringNgoId,
                report.Target.RecordType,
                report.Target.RecordId,
                status,
                actor.Id,
                now));
        }
        else
        {
            row.Change(status, actor.Id, now);
        }

        await reviewService.AddActivityAsync(report,
            ReviewActivity.FollowUpStatusChanged(report.Target, actor, status.Name, now),
            ct);
        await context.SaveChangesAsync(ct);

        if (status.LegacyStatus is not null)
        {
            await SyncRecordColumnAsync(report.Target, status.LegacyStatus, ct);
        }
    }

    /// <summary>The report's own follow-up column, for the NGO that owns it (as the old status endpoints allowed).</summary>
    private async Task SyncRecordColumnAsync(ReviewTarget target, string legacyStatus, CancellationToken ct)
    {
        const string byObserver =
            """ AND "MonitoringObserverId" IN (SELECT "Id" FROM "MonitoringObservers" WHERE "MonitoringNgoId" = @monitoringNgoId)""";
        const string byForm = """ AND "FormId" IN (SELECT "Id" FROM "Forms" WHERE "MonitoringNgoId" = @monitoringNgoId)""";
        string[] tables = target.RecordType == ReportRecordType.FormSubmission
            ? [$"""UPDATE "FormSubmissions" SET "FollowUpStatus" = @status WHERE "Id" = @recordId{byObserver}""",
                $"""UPDATE "PollingStationInformation" SET "FollowUpStatus" = @status WHERE "Id" = @recordId{byObserver}"""]
            : target.RecordType == ReportRecordType.QuickReport
                ? [$"""UPDATE "QuickReports" SET "FollowUpStatus" = @status WHERE "Id" = @recordId{byObserver}"""]
                : target.RecordType == ReportRecordType.IncidentReport
                    ? [$"""UPDATE "IncidentReports" SET "FollowUpStatus" = @status WHERE "Id" = @recordId{byForm}{byObserver}"""]
                    : [$"""UPDATE "CitizenReports" SET "FollowUpStatus" = @status WHERE "Id" = @recordId{byForm}"""];

        foreach (var sql in tables)
        {
            await context.Database.ExecuteSqlRawAsync(sql,
                [
                    new NpgsqlParameter("status", legacyStatus),
                    new NpgsqlParameter("recordId", target.RecordId),
                    new NpgsqlParameter("monitoringNgoId", target.MonitoringNgoId)
                ],
                ct);
        }
    }

    public async Task MaterializeDefaultAsync(Guid electionRoundId, Guid ngoId, FollowUpStatusDefinition previousDefault,
        CancellationToken ct)
    {
        foreach (var recordType in ReportRecordType.List)
        {
            await context.Database.ExecuteSqlRawAsync(
                """
                INSERT INTO "RecordFollowUps" ("Id", "ElectionRoundId", "MonitoringNgoId", "RecordType", "RecordId", "FollowUpStatusId", "ChangedAt", "CreatedOn", "CreatedBy", "LastModifiedBy")
                SELECT gen_random_uuid(), @electionRoundId, @monitoringNgoId, @recordType, V."RecordId", @statusId, @now, @now, '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000'
                FROM "GetVisibleRecordIds"(@electionRoundId, @ngoId, @recordType) V
                ON CONFLICT ("MonitoringNgoId", "RecordType", "RecordId") DO NOTHING
                """,
                [
                    new NpgsqlParameter("electionRoundId", electionRoundId),
                    new NpgsqlParameter("ngoId", ngoId),
                    new NpgsqlParameter("monitoringNgoId", previousDefault.MonitoringNgoId),
                    new NpgsqlParameter("recordType", recordType.Value),
                    new NpgsqlParameter("statusId", previousDefault.Id),
                    new NpgsqlParameter("now", timeProvider.UtcNow)
                ],
                ct);
        }
    }
}
