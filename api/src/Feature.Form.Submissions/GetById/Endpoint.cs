using Module.Answers.Models;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.Form.Submissions.GetById;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFileStorageService fileStorageService) : Endpoint<Request, Results<Ok<FormSubmissionView>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Summary(s => { s.Summary = "Gets submission by id"; });

        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<Ok<FormSubmissionView>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminOrStaffRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT * FROM "GetFormSubmissionView"(@electionRoundId, @ngoId, @submissionId)
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId, ngoId = req.NgoId, submissionId = req.SubmissionId
        };

        FormSubmissionView submission = null;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            submission = await dbConnection.QueryFirstOrDefaultAsync<FormSubmissionView>(sql, queryArgs);
        }

        if (submission is null)
        {
            return TypedResults.NotFound();
        }

        submission = submission with
        {
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
