using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.Form.Submissions.GetById;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFileStorageService fileStorageService) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Summary(s => { s.Summary = "Gets submission by id"; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
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
                      COALESCE(psi."LastModifiedOn", psi."CreatedOn") "TimeSubmitted",
                      psi."ArrivalTime",
                      psi."DepartureTime",
                      psi."Breaks",
                      psi."IsCompleted"
                      FROM "PollingStationInformation" psi
                      INNER JOIN "MonitoringObservers" mo ON mo."Id" = psi."MonitoringObserverId"
                      INNER JOIN "MonitoringNgos" mn ON mn."Id" = mo."MonitoringNgoId"
                      WHERE mn."ElectionRoundId" = @electionRoundId
                          AND mn."NgoId" = @ngoId
                          AND psi."Id" = @submissionId
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
                          COALESCE((select jsonb_agg(jsonb_build_object('QuestionId', "QuestionId", 'FileName', "FileName", 'MimeType', "MimeType", 'FilePath', "FilePath", 'UploadedFileName', "UploadedFileName", 'TimeSubmitted', COALESCE("LastModifiedOn", "CreatedOn")))
                          FROM "Attachments" a
                          WHERE 
                              a."ElectionRoundId" = @electionRoundId
                              AND a."FormId" = fs."FormId"
                              AND a."MonitoringObserverId" = fs."MonitoringObserverId"
                              AND a."IsDeleted" = false AND a."IsCompleted" = true
                              AND fs."PollingStationId" = a."PollingStationId"),'[]'::JSONB) AS "Attachments",
                  
                          COALESCE((select jsonb_agg(jsonb_build_object('QuestionId', "QuestionId", 'Text', "Text", 'TimeSubmitted', COALESCE("LastModifiedOn", "CreatedOn")))
                          FROM "Notes" n
                          WHERE 
                              n."ElectionRoundId" = @electionRoundId
                              AND n."FormId" = fs."FormId"
                              AND n."MonitoringObserverId" = fs."MonitoringObserverId"
                              AND fs."PollingStationId" = n."PollingStationId"), '[]'::JSONB) AS "Notes",
                              
                          COALESCE(fs."LastModifiedOn", fs."CreatedOn") "TimeSubmitted",
                          NULL AS "ArrivalTime",
                          NULL AS "DepartureTime",
                          '[]'::jsonb AS "Breaks",
                          fs."IsCompleted"
                  FROM "FormSubmissions" fs
                  INNER JOIN "MonitoringObservers" mo ON fs."MonitoringObserverId" = mo."Id"
                  INNER JOIN "MonitoringNgos" mn ON mn."Id" = mo."MonitoringNgoId"
                  INNER JOIN "Forms" f ON f."Id" = fs."FormId"
                  WHERE mn."ElectionRoundId" = @electionRoundId
                      AND mn."NgoId" = @ngoId
                      AND fs."Id" = @submissionId)
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
                         u."DisplayName" "ObserverName",
                         u."Email",
                         u."PhoneNumber",
                         mo."Tags",
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
                  INNER JOIN "MonitoringObservers" mo ON mo."Id" = s."MonitoringObserverId"
                  INNER JOIN "MonitoringNgos" mn ON mn."Id" = mo."MonitoringNgoId"
                  INNER JOIN "Observers" o ON o."Id" = mo."ObserverId"
                  INNER JOIN "AspNetUsers" u ON u."Id" = o."ApplicationUserId"
                  WHERE mn."ElectionRoundId" = @electionRoundId
                      AND mn."NgoId" = @ngoId
                  ORDER BY "TimeSubmitted" desc
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            submissionId = req.SubmissionId
        };

        Response submission = null;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            submission = await dbConnection.QueryFirstOrDefaultAsync<Response>(sql, queryArgs);
        }

        if (submission is null)
        {
            return TypedResults.NotFound();
        }

        foreach (var attachment in submission.Attachments)
        {
            var result =
                await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);
            if (result is GetPresignedUrlResult.Ok(var url, _, var urlValidityInSeconds))
            {
                attachment.PresignedUrl = url;
                attachment.UrlValidityInSeconds = urlValidityInSeconds;
            }
        }

        return TypedResults.Ok(submission);
    }
}
