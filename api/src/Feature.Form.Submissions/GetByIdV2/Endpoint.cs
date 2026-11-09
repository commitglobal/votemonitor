using Module.Answers.Models;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Feature.Form.Submissions.GetByIdV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFileStorageService fileStorageService,
    VoteMonitorContext context) : Endpoint<Request, Results<Ok<FormSubmissionViewV2>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}:v2");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Summary(s => { s.Summary = "Gets submission by id"; });

        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<Ok<FormSubmissionViewV2>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT * FROM "GetFormSubmissionViewV2"(@electionRoundId, @ngoId, @submissionId)
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId, ngoId = req.NgoId, submissionId = req.SubmissionId
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

        var followUp = await context.GetFollowUpStatusAsync(req.ElectionRoundId, req.NgoId, ReportRecordType.FormSubmission,
            req.SubmissionId, ct);
        submission = submission with
        {
            FollowUpStatusId = followUp?.Id,
            FollowUpStatusName = followUp?.Name,
            FollowUpStatusColor = followUp?.Color,
            FollowUpStatusIcon = followUp?.Icon,
            Attachments = await Task.WhenAll(
                submission.Attachments.Select(async attachment =>
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
