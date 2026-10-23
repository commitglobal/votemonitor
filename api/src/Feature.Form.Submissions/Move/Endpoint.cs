using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.PollingStationCorrectionAggregate;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.Form.Submissions.Move;

public class Endpoint(
    VoteMonitorContext context,
    ICurrentUserProvider userProvider,
    IRecordAutomation recordAutomation,
    ITimeProvider timeProvider)
    : Endpoint<Request, Results<NoContent, NotFound, ValidationProblem, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}:move");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s => { s.Summary = "Moves a form submission to another polling station (platform admins)"; });
    }

    public override async Task<Results<NoContent, NotFound, ValidationProblem, Conflict<ProblemDetails>>> ExecuteAsync(
        Request req, CancellationToken ct)
    {
        var submission = await context.FormSubmissions
            .FirstOrDefaultAsync(x => x.ElectionRoundId == req.ElectionRoundId && x.Id == req.SubmissionId, ct);
        if (submission is null)
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

        var previousPollingStationId = submission.PollingStationId;
        if (previousPollingStationId == pollingStation.Id)
        {
            return TypedResults.NoContent();
        }

        var hasConflictingSubmission = await context.FormSubmissions
            .AnyAsync(x => x.ElectionRoundId == submission.ElectionRoundId
                           && x.Id != submission.Id
                           && x.MonitoringObserverId == submission.MonitoringObserverId
                           && x.FormId == submission.FormId
                           && x.PollingStationId == pollingStation.Id
                           && !x.MonitoringObserver.MonitoringNgo.AllowMultipleFormSubmission, ct);
        if (hasConflictingSubmission)
        {
            AddError(x => x.PollingStationId,
                "The observer already has a submission of this form at the target polling station");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures, StatusCodes.Status409Conflict));
        }

        var strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(ct);

            // Notes and attachments carry the polling station too; legacy rows are linked by form + polling station + observer instead of submission id.
            await context.Notes
                .Where(n => n.ElectionRoundId == submission.ElectionRoundId
                            && (n.SubmissionId == submission.Id
                                || (n.FormId == submission.FormId
                                    && n.PollingStationId == previousPollingStationId
                                    && n.MonitoringObserverId == submission.MonitoringObserverId
                                    && !context.FormSubmissions.Any(fs => fs.Id == n.SubmissionId))))
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.PollingStationId, pollingStation.Id), ct);

            await context.Attachments
                .Where(a => a.ElectionRoundId == submission.ElectionRoundId
                            && (a.SubmissionId == submission.Id
                                || (a.FormId == submission.FormId
                                    && a.PollingStationId == previousPollingStationId
                                    && a.MonitoringObserverId == submission.MonitoringObserverId
                                    && !context.FormSubmissions.Any(fs => fs.Id == a.SubmissionId))))
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.PollingStationId, pollingStation.Id), ct);

            submission.MoveToPollingStation(pollingStation);
            context.PollingStationCorrections.Add(PollingStationCorrection.Create(submission.ElectionRoundId,
                CorrectableRecordType.FormSubmission,
                submission.Id,
                previousPollingStationId,
                pollingStation.Id,
                timeProvider.UtcNow,
                userProvider.GetUserId()!.Value));
            await context.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
        });

        await recordAutomation.RecordEditedAsync(submission.ElectionRoundId, ReportRecordType.FormSubmission,
            submission.Id, ct);

        return TypedResults.NoContent();
    }
}
