using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.PollingStationCorrectionAggregate;

namespace Feature.IncidentReports.Move;

public class Endpoint(VoteMonitorContext context, ICurrentUserProvider userProvider, ITimeProvider timeProvider)
    : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/incident-reports/{incidentReportId}:move");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s => { s.Summary = "Moves an incident report to another polling station (platform admins)"; });
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        // Only incident reports at a polling station are correctable.
        var incidentReport = await context.IncidentReports
            .FirstOrDefaultAsync(x => x.ElectionRoundId == req.ElectionRoundId
                                      && x.Id == req.IncidentReportId
                                      && x.PollingStationId != null, ct);
        if (incidentReport is null)
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

        var previousPollingStationId = incidentReport.PollingStationId!.Value;
        if (previousPollingStationId == pollingStation.Id)
        {
            return TypedResults.NoContent();
        }

        incidentReport.MoveToPollingStation(pollingStation);
        context.PollingStationCorrections.Add(PollingStationCorrection.Create(incidentReport.ElectionRoundId,
            CorrectableRecordType.IncidentReport,
            incidentReport.Id,
            previousPollingStationId,
            pollingStation.Id,
            timeProvider.UtcNow,
            userProvider.GetUserId()!.Value));
        await context.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
