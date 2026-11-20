using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Entities.RecordLabelAggregate;
using Vote.Monitor.Domain.Entities.RecordSeverityAggregate;
using Microsoft.EntityFrameworkCore;
using Module.Answers.Models;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.Form.Submissions.GetByIdV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    ICurrentUserRoleProvider userRoleProvider,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFileStorageService fileStorageService,
    VoteMonitorContext context) : Endpoint<Request, Results<Ok<FormSubmissionViewV2>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}:v2");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Summary(s =>
        {
            s.Summary = "Gets submission by id";
            s.Description = "Platform admins get who sent it, from where and its corrections, never what was reported";
        });

        Policies(PolicyNames.NotObservers);
    }

    public override async Task<Results<Ok<FormSubmissionViewV2>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var isPlatformAdmin = userRoleProvider.IsPlatformAdmin();
        Guid ngoId;
        if (isPlatformAdmin)
        {
            // Read as the NGO whose observer sent it; ForPlatformAdmin then leaves out what platform admins mustn't see.
            var owner = await context.GetRecordOwnerAsync(req.ElectionRoundId, ReportRecordType.FormSubmission,
                req.SubmissionId, ct);
            if (owner is null)
            {
                return TypedResults.NotFound();
            }

            ngoId = owner.NgoId;
        }
        else
        {
            var authorizationResult =
                await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
            if (!authorizationResult.Succeeded || req.NgoId is not { } callerNgoId || callerNgoId == Guid.Empty)
            {
                return TypedResults.NotFound();
            }

            ngoId = callerNgoId;
        }

        var sql = """
                  SELECT * FROM "GetFormSubmissionViewV2"(@electionRoundId, @ngoId, @submissionId)
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId, ngoId, submissionId = req.SubmissionId
        };

        FormSubmissionViewV2 submission = null;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            submission = await dbConnection.QueryFirstOrDefaultAsync<FormSubmissionViewV2>(sql, queryArgs);
        }

        if (submission is null)
        {
            return TypedResults.NotFound();
        }

        // The stored counts (display logic applied); the view function doesn't return them.
        var counts = await context.FormSubmissions
                         .Where(x => x.Id == req.SubmissionId && x.ElectionRoundId == req.ElectionRoundId)
                         .Select(x => new { x.NumberOfQuestionsAnswered, x.NumberOfUnansweredQuestions })
                         .FirstOrDefaultAsync(ct)
                     ?? await context.PollingStationInformation
                         .Where(x => x.Id == req.SubmissionId && x.ElectionRoundId == req.ElectionRoundId)
                         .Select(x => new { x.NumberOfQuestionsAnswered, x.NumberOfUnansweredQuestions })
                         .FirstOrDefaultAsync(ct);

        var address = await context.PollingStations
            .Where(x => x.Id == submission.PollingStationId)
            .Select(x => x.Address)
            .FirstOrDefaultAsync(ct);
        if (isPlatformAdmin)
        {
            return TypedResults.Ok(submission.ForPlatformAdmin() with { Address = address });
        }

        var followUp = await context.GetFollowUpStatusAsync(req.ElectionRoundId, ngoId, ReportRecordType.FormSubmission,
            req.SubmissionId, ct);
        var reviewTarget = await context.GetReviewTargetAsync(req.ElectionRoundId, ngoId, ReportRecordType.FormSubmission, req.SubmissionId, ct);
        var reviewWorkflow = reviewTarget is null ? null : await context.GetReviewWorkflowAsync(reviewTarget, ct);
        var recordSeverity = reviewTarget is null ? null : await context.GetRecordSeverityAsync(reviewTarget, ct);
        var recordLabels = reviewTarget is null ? null : await context.GetRecordLabelsAsync(reviewTarget, ct);
        submission = submission with
        {
            Address = address,
            NumberOfQuestionsAnswered = counts?.NumberOfQuestionsAnswered ?? submission.NumberOfQuestionsAnswered,
            NumberOfUnansweredQuestions = counts?.NumberOfUnansweredQuestions ?? 0,
            FollowUpStatusId = followUp?.Id,
            FollowUpStatusName = followUp?.Name,
            FollowUpStatusColor = followUp?.Color,
            FollowUpStatusIcon = followUp?.Icon,
            ReviewStage = reviewWorkflow?.Stage,
            StageChangedAt = reviewWorkflow?.StageChangedAt,
            ReviewedAt = reviewWorkflow?.ReviewedAt,
            Reviewers = reviewWorkflow?.Reviewers,
            Severity = recordSeverity?.Severity,
            SeverityChangedAt = recordSeverity?.ChangedAt,
            Labels = recordLabels,
            Attachments = await Task.WhenAll(
                (submission.Attachments ?? []).Select(async attachment =>
                {
                    var result =
                        await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);
                    return result is GetPresignedUrlResult.Ok(var url, _, var urlValidityInSeconds)
                        ? attachment with { PresignedUrl = url, UrlValidityInSeconds = urlValidityInSeconds }
                        : attachment;
                })
            )
        };

        return TypedResults.Ok(submission);
    }
}
