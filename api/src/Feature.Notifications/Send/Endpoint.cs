using Authorization.Policies;
using Dapper;
using Feature.Notifications.Specifications;
using Job.Contracts;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Notifications.Send;

public class Endpoint(
    IRepository<NotificationAggregate> repository,
    IReadRepository<MonitoringObserver> monitoringObserverRepository,
    INpgsqlConnectionFactory dbConnectionFactory,
    IJobService jobService,
    IHtmlStringSanitizer htmlStringSanitizer) :
    Endpoint<Request, Results<Ok<Response>, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/notifications:send");
        DontAutoTag();
        Options(x => x.WithTags("notifications"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, ProblemHttpResult>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = """
                  WITH "ObserverPSI" AS
                    (SELECT f."Id" AS "FormId",
                            f."FormType" AS "FormType",
                            mo."ObserverId" AS "ObserverId",
                            mo."Id" AS "MonitoringObserverId",
                            fs."PollingStationId" AS "PollingStationId",
                            fs."FollowUpStatus" AS "FollowUpStatus",
                            COALESCE(FS."LastModifiedOn", FS."CreatedOn") AS "LastModifiedOn",
                            fs."IsCompleted" AS "IsCompleted",
                            CAST(NULL AS bigint) AS "MediaFilesCount",
                            CAST(NULL AS bigint) AS "NotesCount",
                            (CASE
                                 WHEN FS."NumberOfQuestionsAnswered" = F."NumberOfQuestions" THEN 'All'
                                 WHEN fs."NumberOfQuestionsAnswered" > 0 THEN 'Some'
                                 WHEN fs."NumberOfQuestionsAnswered" = 0 THEN 'None'
                             END) "QuestionsAnswered",
                            (CASE
                                 WHEN FS."NumberOfFlaggedAnswers" > 0 THEN TRUE
                                 ELSE FALSE
                             END) "HasFlaggedAnswers",
                            CAST(NULL AS UUID) AS "QuickReportId",
                            NULL AS "IncidentCategory",
                            NULL AS "QuickReportFollowUpStatus"
                     FROM "MonitoringObservers" MO
                     INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                     INNER JOIN "PollingStationInformation" FS ON MO."Id" = FS."MonitoringObserverId"
                     INNER JOIN "PollingStationInformationForms" F ON f."ElectionRoundId" = @electionRoundId
                     WHERE MN."ElectionRoundId" = @electionRoundId
                       AND MN."NgoId" = @ngoId),
                       "ObserversFormSubmissions" AS
                    (SELECT f."Id" AS "FormId",
                            f."FormType" AS "FormType",
                            mo."ObserverId" AS "ObserverId",
                            mo."Id" AS "MonitoringObserverId",
                            fs."PollingStationId" AS "PollingStationId",
                            fs."FollowUpStatus" AS "FollowUpStatus",
                            COALESCE(FS."LastModifiedOn", FS."CreatedOn") AS "LastModifiedOn",
                            fs."IsCompleted" AS "IsCompleted",
                  
                       (SELECT COUNT(*)
                        FROM "Attachments" A
                        WHERE A."FormId" = fs."FormId"
                          AND a."MonitoringObserverId" = fs."MonitoringObserverId"
                          AND fs."PollingStationId" = A."PollingStationId"
                          AND A."IsDeleted" = FALSE
                          AND A."IsCompleted" = TRUE) AS "MediaFilesCount",
                  
                       (SELECT COUNT(*)
                        FROM "Notes" N
                        WHERE N."FormId" = fs."FormId"
                          AND N."MonitoringObserverId" = fs."MonitoringObserverId"
                          AND fs."PollingStationId" = N."PollingStationId") AS "NotesCount",
                            (CASE
                                 WHEN FS."NumberOfQuestionsAnswered" = F."NumberOfQuestions" THEN 'ALL'
                                 WHEN fs."NumberOfQuestionsAnswered" > 0 THEN 'SOME'
                                 WHEN fs."NumberOfQuestionsAnswered" = 0 THEN 'NONE'
                             END) "QuestionsAnswered",
                            (CASE
                                 WHEN FS."NumberOfFlaggedAnswers" > 0 THEN TRUE
                                 ELSE FALSE
                             END) "HasFlaggedAnswers",
                            CAST(NULL AS UUID) AS "QuickReportId",
                            NULL AS "IncidentCategory",
                            NULL AS "QuickReportFollowUpStatus"
                     FROM "MonitoringObservers" MO
                     INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                     INNER JOIN "FormSubmissions" FS ON MO."Id" = FS."MonitoringObserverId"
                     INNER JOIN "Forms" F ON FS."FormId" = F."Id"
                     WHERE MN."ElectionRoundId" = @electionRoundId
                       AND MN."NgoId" = @ngoId),
                       "ObserversQuickReports" AS
                    (SELECT CAST(NULL AS UUID) AS "FormId",
                            NULL AS "FormType",
                            mo."ObserverId" AS "ObserverId",
                            mo."Id" AS "MonitoringObserverId",
                            qr."PollingStationId" AS "PollingStationId",
                            NULL AS "FollowUpStatus",
                            COALESCE(qr."LastModifiedOn", qr."CreatedOn") AS "LastModifiedOn",
                            CAST(NULL AS boolean) AS "IsCompleted",
                            CAST(NULL AS bigint) AS "MediaFilesCount",
                            CAST(NULL AS bigint) AS "NotesCount",
                            NULL AS "QuestionsAnswered",
                            CAST(NULL AS boolean) AS "HasFlaggedAnswers",
                            qr."Id" AS "QuickReportId",
                            qr."IncidentCategory" AS "IncidentCategory",
                            qr."FollowUpStatus" AS "QuickReportFollowUpStatus"
                     FROM "MonitoringObservers" MO
                     INNER JOIN "MonitoringNgos" MN ON MN."Id" = MO."MonitoringNgoId"
                     INNER JOIN "QuickReports" QR ON MO."Id" = QR."MonitoringObserverId"
                     WHERE MN."ElectionRoundId" = @electionRoundId
                       AND MN."NgoId" = @ngoId),
                       "ObserversActivity" AS
                    (SELECT *
                     FROM "ObserversFormSubmissions"
                     UNION ALL SELECT *
                     FROM "ObserversQuickReports"
                     UNION ALL SELECT *
                     FROM "ObserverPSI")
                  SELECT DISTINCT OA."MonitoringObserverId",
                                  NT."Token"
                  FROM "ObserversActivity" OA
                  INNER JOIN "MonitoringObservers" mo ON mo."Id" = OA."MonitoringObserverId"
                  INNER JOIN "AspNetUsers" U ON U."Id" = OA."ObserverId"
                  LEFT JOIN "PollingStations" ps ON OA."PollingStationId" = ps."Id"
                  LEFT JOIN "NotificationTokens" NT ON NT."ObserverId" = OA."ObserverId"
                  WHERE (@searchText IS NULL
                         OR @searchText = ''
                         OR U."DisplayName" ILIKE @searchText
                         OR U."Email" ILIKE @searchText
                         OR u."PhoneNumber" ILIKE @searchText
                         OR mo."Id"::text ILIKE @searchText)
                    AND (@tagsFilter IS NULL
                         OR cardinality(@tagsFilter) = 0
                         OR mo."Tags" && @tagsFilter)
                    AND (@monitoringObserverStatus IS NULL
                         OR mo."Status" = @monitoringObserverStatus)
                    AND (@formType IS NULL
                         OR OA."FormType" = @formType)
                    AND (@level1 IS NULL
                         OR ps."Level1" = @level1)
                    AND (@level2 IS NULL
                         OR ps."Level2" = @level2)
                    AND (@level3 IS NULL
                         OR ps."Level3" = @level3)
                    AND (@level4 IS NULL
                         OR ps."Level4" = @level4)
                    AND (@level5 IS NULL
                         OR ps."Level5" = @level5)
                    AND (@pollingStationNumber IS NULL
                         OR ps."Number" = @pollingStationNumber)
                    AND (@hasFlaggedAnswers IS NULL
                         OR OA."HasFlaggedAnswers" = @hasFlaggedAnswers)
                    AND (@submissionsFollowUpStatus IS NULL
                         OR OA."FollowUpStatus" = @submissionsFollowUpStatus)
                    AND (@formId IS NULL
                         OR OA."FormId" = @formId)
                    AND (@questionsAnswered IS NULL
                         OR OA."QuestionsAnswered" = @questionsAnswered)
                    AND (@hasAttachments IS NULL
                         OR (@hasAttachments = TRUE
                             AND OA."MediaFilesCount" > 0)
                         OR (@hasAttachments = FALSE
                             AND OA."MediaFilesCount" = 0))
                    AND (@hasNotes IS NULL
                         OR (OA."NotesCount" = 0
                             AND @hasNotes = FALSE)
                         OR (OA."NotesCount" > 0
                             AND @hasNotes = TRUE))
                    AND (@fromDate IS NULL
                         OR OA."LastModifiedOn" >= @fromDate::timestamp)
                    AND (@toDate IS NULL
                         OR OA."LastModifiedOn" <= @toDate::timestamp)
                    AND (@isCompleted IS NULL
                         OR OA."IsCompleted" = @isCompleted)
                    AND (@hasQuickReports IS NULL
                         OR (@hasQuickReports = TRUE
                             AND OA."QuickReportId" IS NOT NULL)
                         OR (@hasQuickReports = FALSE
                             AND OA."QuickReportId" IS NULL))
                    AND (@quickReportFollowUpStatus IS NULL
                         OR OA."QuickReportFollowUpStatus" = @quickReportFollowUpStatus)
                    AND (@quickReportIncidentCategory IS NULL
                         OR OA."IncidentCategory" = @quickReportIncidentCategory)
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            formType = req.FormTypeFilter?.ToString(),
            level1 = req.Level1Filter,
            level2 = req.Level2Filter,
            level3 = req.Level3Filter,
            level4 = req.Level4Filter,
            level5 = req.Level5Filter,
            pollingStationNumber = req.PollingStationNumberFilter,
            hasFlaggedAnswers = req.HasFlaggedAnswers,
            submissionsFollowUpStatus = req.SubmissionsFollowUpStatus?.ToString(),
            tagsFilter = req.TagsFilter ?? [],
            monitoringObserverStatus = req.MonitoringObserverStatus?.ToString(),
            formId = req.FormId,
            hasNotes = req.HasNotes,
            hasAttachments = req.HasAttachments,
            questionsAnswered = req.QuestionsAnswered?.ToString(),
            fromDate = req.FromDateFilter?.ToString("O"),
            toDate = req.ToDateFilter?.ToString("O"),
            isCompleted = req.IsCompletedFilter,

            hasQuickReports = req.HasQuickReports,
            quickReportFollowUpStatus = req.QuickReportFollowUpStatus?.ToString(),
            quickReportIncidentCategory = req.QuickReportIncidentCategory?.ToString()
        };

        IEnumerable<NotificationRecipient> result = [];
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            result = await dbConnection.QueryAsync<NotificationRecipient>(sql, queryArgs);
        }

        var recipients = result.ToList();

        var monitoringObserverIds = recipients.Select(x => x.MonitoringObserverId).Distinct().ToList();
        var pushNotificationTokens = recipients
            .Select(x => x.Token)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!)
            .Distinct()
            .ToList();

        var monitoringObservers = await monitoringObserverRepository.ListAsync(
            new GetMonitoringObserverSpecification(req.ElectionRoundId, req.NgoId, monitoringObserverIds), ct);

        var sanitizedMessage = htmlStringSanitizer.Sanitize(req.Body);

        var notification = NotificationAggregate.Create(req.ElectionRoundId,
            req.UserId,
            monitoringObservers,
            req.Title,
            sanitizedMessage);

        await repository.AddAsync(notification, ct);

        jobService.EnqueueSendNotifications(pushNotificationTokens, req.Title, sanitizedMessage);

        return TypedResults.Ok(new Response
        {
            Status = "Success"
        });
    }
}
