using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.IncidentReports.Unlock;

/// <summary>
/// Lets the observer change a completed incident report again. Completing a report locks it: the observer's upsert is
/// refused until it is unlocked. The NGO admins and staff of the observer's own monitoring NGO can unlock it, never in an
/// archived round; each unlock of a locked report goes in its review history (who and when).
/// </summary>
public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context, ITimeProvider timeProvider)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/incident-reports/{id}:unlock");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports"));
        Summary(s =>
        {
            s.Summary = "Unlocks a completed incident report so its observer can change it again";
            s.Description = "NGO admins and staff of the observer's monitoring NGO; not in archived rounds (400). " +
                            "The unlock goes in the report's review history (UnlockedForEditing). A report that isn't " +
                            "locked stays as it is, with no history entry. Another NGO's report is 404.";
        });
        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var report = context.IncidentReports
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.MonitoringNgo.NgoId == req.NgoId
                        && x.Id == req.Id);
        if (!await report.AnyAsync(ct))
        {
            return TypedResults.NotFound();
        }

        if (await context.ElectionRounds.AnyAsync(x => x.Id == req.ElectionRoundId
                                                       && x.Status == ElectionRoundStatus.Archived, ct))
        {
            ThrowError("The election round is archived");
        }

        var unlocked = await report.Where(x => x.IsCompleted)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsCompleted, false), ct);
        if (unlocked == 0)
        {
            return TypedResults.NoContent();
        }

        var monitoringNgoId = await context.GetMonitoringNgoIdAsync(req.ElectionRoundId, req.NgoId, ct);
        var actorName = await context.Users.Where(x => x.Id == req.UserId).Select(x => x.DisplayName).FirstOrDefaultAsync(ct);
        if (monitoringNgoId is not null && actorName is not null)
        {
            context.ReviewActivities.Add(ReviewActivity.UnlockedForEditing(
                new ReviewTarget(req.ElectionRoundId, monitoringNgoId.Value, ReportRecordType.IncidentReport, req.Id),
                new ReviewActor(req.UserId, actorName),
                timeProvider.UtcNow));
            await context.SaveChangesAsync(ct);
        }

        return TypedResults.NoContent();
    }
}
