using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Feature.IncidentReports.Unlock;

/// <summary>
/// Lets the observer change a completed incident report again. Completing a report locks it: the observer's upsert is
/// refused until it is unlocked. Only the NGO admins of the observer's own monitoring NGO can unlock it, never in an
/// archived round.
/// </summary>
public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
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
            s.Description = "NGO admins of the observer's monitoring NGO; not in archived rounds (400). A report " +
                            "that isn't locked stays as it is. Another NGO's report is 404.";
        });
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
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

        await report.ExecuteUpdateAsync(x => x.SetProperty(p => p.IsCompleted, false), ct);
        return TypedResults.NoContent();
    }
}
