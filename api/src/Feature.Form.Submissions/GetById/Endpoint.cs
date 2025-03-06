﻿using Vote.Monitor.Answer.Module.Models;
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

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<FormSubmissionView>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  WITH submissions AS
                  (SELECT psi."Id" AS "SubmissionId",
                      'PSI' AS "FormType",
                      'PSI' AS "FormCode",
                      psi."PollingStationId",
                      psi."MonitoringObserverId",
                      psi."Answers",
                      (SELECT "Questions"
                      FROM "PollingStationInformationForms"
                      WHERE "ElectionRoundId" = @electionRoundId) AS "Questions",
                      (SELECT "DefaultLanguage"
                      FROM "PollingStationInformationForms"
                      WHERE "ElectionRoundId" = @electionRoundId) AS "DefaultLanguage",
                      psi."FollowUpStatus" as "FollowUpStatus",
                      '[]'::jsonb AS "Attachments",
                      '[]'::jsonb AS "Notes",
                      "LastUpdatedAt" AS "TimeSubmitted",
                      psi."ArrivalTime",
                      psi."DepartureTime",
                      psi."Breaks",
                      psi."IsCompleted"
                      FROM "PollingStationInformation" psi
                      INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, 'Coalition') AMO on AMO."MonitoringObserverId" = psi."MonitoringObserverId"
                      WHERE psi."Id" = @submissionId and psi."ElectionRoundId" = @electionRoundId
                  UNION ALL
                  SELECT 
                          fs."Id" AS "SubmissionId",
                          f."FormType" AS "FormType",
                          f."Code" AS "FormCode",
                          fs."PollingStationId",
                          fs."MonitoringObserverId",
                          fs."Answers",
                          f."Questions",
                          f."DefaultLanguage",
                          fs."FollowUpStatus",
                          COALESCE((select jsonb_agg(jsonb_build_object('QuestionId', "QuestionId", 'FileName', "FileName", 'MimeType', "MimeType", 'FilePath', "FilePath", 'UploadedFileName', "UploadedFileName", 'TimeSubmitted', "LastUpdatedAt"))
                          FROM "Attachments" a
                          WHERE 
                              a."ElectionRoundId" = @electionRoundId
                              AND a."FormId" = fs."FormId"
                              AND a."MonitoringObserverId" = fs."MonitoringObserverId"
                              AND a."IsDeleted" = false AND a."IsCompleted" = true
                              AND fs."PollingStationId" = a."PollingStationId"),'[]'::JSONB) AS "Attachments",
                  
                          COALESCE((select jsonb_agg(jsonb_build_object('QuestionId', "QuestionId", 'Text', "Text", 'TimeSubmitted', "LastUpdatedAt"))
                          FROM "Notes" n
                          WHERE 
                              n."ElectionRoundId" = @electionRoundId
                              AND n."FormId" = fs."FormId"
                              AND n."MonitoringObserverId" = fs."MonitoringObserverId"
                              AND fs."PollingStationId" = n."PollingStationId"), '[]'::JSONB) AS "Notes",
                              
                          "LastUpdatedAt" AS "TimeSubmitted",
                          NULL AS "ArrivalTime",
                          NULL AS "DepartureTime",
                          '[]'::jsonb AS "Breaks",
                          fs."IsCompleted"
                  FROM "FormSubmissions" fs
                  INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, 'Coalition') AMO on AMO."MonitoringObserverId" = FS."MonitoringObserverId"
                  INNER JOIN "Forms" f ON f."Id" = fs."FormId"
                  WHERE fs."Id" = @submissionId and fs."ElectionRoundId" = @electionRoundId)
                  SELECT s."SubmissionId",
                         s."TimeSubmitted",
                         s."FormCode",
                         s."FormType",
                         ps."Id" AS "PollingStationId",
                         ps."Level1",
                         ps."Level2",
                         ps."Level3",
                         ps."Level4",
                         ps."Level5",
                         ps."Number",
                         s."MonitoringObserverId",
                         AMO."DisplayName" "ObserverName",
                         AMO."Email",
                         AMO."PhoneNumber",
                         AMO."Tags",
                         AMO."NgoName",
                         AMO."IsOwnObserver",
                         s."Attachments",
                         s."Notes",
                         s."Answers",
                         s."Questions",
                         s."DefaultLanguage",
                         s."FollowUpStatus",
                         s."ArrivalTime",
                         s."DepartureTime",
                         s."Breaks",
                         s."IsCompleted"
                  FROM submissions s
                  INNER JOIN "PollingStations" ps ON ps."Id" = s."PollingStationId"
                  INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, 'Coalition') AMO on AMO."MonitoringObserverId" = s."MonitoringObserverId"
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
