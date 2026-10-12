using Authorization.Policies;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.PollingStationCorrectionAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.Move;

public class Endpoint(VoteMonitorContext context, ICurrentUserProvider userProvider, ITimeProvider timeProvider)
    : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/quick-reports/{id}:move");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s => { s.Summary = "Moves a quick report to another polling station (platform admins)"; });
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        // Only quick reports from a visited polling station are correctable.
        var quickReport = await context.QuickReports
            .FirstOrDefaultAsync(x => x.ElectionRoundId == req.ElectionRoundId
                                      && x.Id == req.Id
                                      && x.QuickReportLocationType == QuickReportLocationType.VisitedPollingStation
                                      && x.PollingStationId != null, ct);
        if (quickReport is null)
        {
            return TypedResults.NotFound();
        }

        var pollingStation = await context.PollingStations
            .FirstOrDefaultAsync(x => x.ElectionRoundId == req.ElectionRoundId && x.Id == req.PollingStationId, ct);
        if (pollingStation is null)
        {
            return TypedResults.NotFound();
        }

        var isArchived = await context.ElectionRounds
            .AnyAsync(x => x.Id == req.ElectionRoundId && x.Status == ElectionRoundStatus.Archived, ct);
        if (isArchived)
        {
            AddError(x => x.ElectionRoundId, "Election round is archived");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        var previousPollingStationId = quickReport.PollingStationId!.Value;
        if (previousPollingStationId == pollingStation.Id)
        {
            return TypedResults.NoContent();
        }

        quickReport.MoveToPollingStation(pollingStation);
        context.PollingStationCorrections.Add(PollingStationCorrection.Create(quickReport.ElectionRoundId,
            CorrectableRecordType.QuickReport,
            quickReport.Id,
            previousPollingStationId,
            pollingStation.Id,
            timeProvider.UtcNow,
            userProvider.GetUserId()!.Value));
        await context.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
