using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Form.Submissions.ListEntriesDetailed;

public class Endpoint(
    IAuthorizationService authorizationService,
    INpgsqlConnectionFactory dbConnectionFactory,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<PagedResponse<DetailedSubmissionEntry>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:byEntryDetailed");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(x => { x.Summary = "Lists form submissions by entry detailed"; });
    }

    public override async Task<Results<Ok<PagedResponse<DetailedSubmissionEntry>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var sql = """
                  SELECT SUM(count)
                  FROM (SELECT count(*) AS count
                        FROM "PollingStationInformation" psi
                                 INNER JOIN "PollingStationInformationForms" psif ON psif."Id" = psi."PollingStationInformationFormId"
                                 INNER JOIN "PollingStations" ps ON ps."Id" = psi."PollingStationId"
                                 INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) mo ON mo."MonitoringObserverId" = psi."MonitoringObserverId"
                        WHERE psi."ElectionRoundId" = @electionRoundId
                          AND (@monitoringObserverId IS NULL OR mo."MonitoringObserverId" = @monitoringObserverId)
                          AND (@COALITIONMEMBERID IS NULL OR mo."NgoId" = @COALITIONMEMBERID)
                          AND (@searchText IS NULL
                            OR @searchText = ''
                            OR mo."DisplayName" ILIKE @searchText
                            OR mo."Email" ILIKE @searchText
                            OR mo."PhoneNumber" ILIKE @searchText
                            OR mo."MonitoringObserverId"::TEXT ILIKE @searchText)
                          AND (@formType IS NULL OR 'PSI' = @formType)
                          AND (@level1 IS NULL OR ps."Level1" = @level1)
                          AND (@level2 IS NULL OR ps."Level2" = @level2)
                          AND (@level3 IS NULL OR ps."Level3" = @level3)
                          AND (@level4 IS NULL OR ps."Level4" = @level4)
                          AND (@level5 IS NULL OR ps."Level5" = @level5)
                          AND (@pollingStationNumber IS NULL OR ps."Number" = @pollingStationNumber)
                          AND (@hasFlaggedAnswers is NULL OR @hasFlaggedAnswers = false OR 1 = 2)
                          AND (@followUpStatus is NULL OR psi."FollowUpStatus" = @followUpStatus)
                          AND (@monitoringObserverStatus IS NULL OR mo."Status" = @monitoringObserverStatus)
                          AND (@formId IS NULL OR psi."PollingStationInformationFormId" = @formId)
                          AND (@questionsAnswered is null
                            OR (@questionsAnswered = 'All' AND psif."NumberOfQuestions" = psi."NumberOfQuestionsAnswered")
                            OR (@questionsAnswered = 'Some' AND psif."NumberOfQuestions" <> psi."NumberOfQuestionsAnswered")
                            OR (@questionsAnswered = 'None' AND psi."NumberOfQuestionsAnswered" = 0))
                          AND (@hasNotes is NULL OR (TRUE AND @hasNotes = false) OR (FALSE AND @hasNotes = true))
                          AND (@hasAttachments is NULL OR (TRUE AND @hasAttachments = false) OR (FALSE AND @hasAttachments = true))
                          AND (@fromDate is NULL OR PSI."LastUpdatedAt" >= @fromDate::timestamp)
                          AND (@toDate is NULL OR PSI."LastUpdatedAt" <= @toDate::timestamp)
                        UNION ALL
                        SELECT count(*) AS count
                        FROM "FormSubmissions" fs
                                 INNER JOIN "Forms" f ON f."Id" = fs."FormId"
                                 INNER JOIN "GetAvailableForms"(@electionRoundId, @ngoId, @dataSource)  AF ON AF."FormId" = fs."FormId"
                                 INNER JOIN "PollingStations" ps ON ps."Id" = fs."PollingStationId"
                                 INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource)  mo ON mo."MonitoringObserverId" = fs."MonitoringObserverId"
                        WHERE fs."ElectionRoundId" = @electionRoundId
                          AND (@monitoringObserverId IS NULL OR mo."MonitoringObserverId" = @monitoringObserverId)
                          AND (@COALITIONMEMBERID IS NULL OR mo."NgoId" = @COALITIONMEMBERID)
                          AND (@searchText IS NULL
                            OR @searchText = ''
                            OR mo."DisplayName" ILIKE @searchText
                            OR mo."Email" ILIKE @searchText
                            OR mo."PhoneNumber" ILIKE @searchText
                            OR mo."MonitoringObserverId"::TEXT ILIKE @searchText)
                          AND (@formType IS NULL OR f."FormType" = @formType)
                          AND (@level1 IS NULL OR ps."Level1" = @level1)
                          AND (@level2 IS NULL OR ps."Level2" = @level2)
                          AND (@level3 IS NULL OR ps."Level3" = @level3)
                          AND (@level4 IS NULL OR ps."Level4" = @level4)
                          AND (@level5 IS NULL OR ps."Level5" = @level5)
                          AND (@pollingStationNumber IS NULL OR ps."Number" = @pollingStationNumber)
                          AND (@hasFlaggedAnswers is NULL OR (fs."NumberOfFlaggedAnswers" = 0 AND @hasFlaggedAnswers = false) OR
                               ("NumberOfFlaggedAnswers" > 0 AND @hasFlaggedAnswers = true))
                          AND (@followUpStatus is NULL OR fs."FollowUpStatus" = @followUpStatus)
                          AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR mo."Tags" && @tagsFilter)
                          AND (@monitoringObserverStatus IS NULL OR mo."Status" = @monitoringObserverStatus)
                          AND (@formId IS NULL OR fs."FormId" = @formId)
                          AND (@questionsAnswered is null
                            OR (@questionsAnswered = 'All' AND f."NumberOfQuestions" = fs."NumberOfQuestionsAnswered")
                            OR (@questionsAnswered = 'Some' AND f."NumberOfQuestions" <> fs."NumberOfQuestionsAnswered")
                            OR (@questionsAnswered = 'None' AND fs."NumberOfQuestionsAnswered" = 0))
                          AND (@hasAttachments is NULL
                            OR ((SELECT COUNT(1)
                                 FROM "Attachments" A
                                 WHERE 
                                 (
                                      (A."FormId" = FS."FormId" AND FS."PollingStationId" = A."PollingStationId") -- backwards compatibility
                                      OR A."SubmissionId" = FS."Id"
                                  )
                                   AND A."MonitoringObserverId" = fs."MonitoringObserverId"
                                   AND A."IsDeleted" = false
                                   AND A."IsCompleted" = true) = 0 AND @hasAttachments = false)
                            OR ((SELECT COUNT(1)
                                 FROM "Attachments" A
                                 WHERE 
                                    (
                                      (A."FormId" = FS."FormId" AND FS."PollingStationId" = A."PollingStationId") -- backwards compatibility
                                      OR A."SubmissionId" = FS."Id"
                                     )
                                   AND A."MonitoringObserverId" = fs."MonitoringObserverId"
                                   AND A."IsDeleted" = false
                                   AND A."IsCompleted" = true) > 0 AND @hasAttachments = true))
                          AND (@hasNotes is NULL
                            OR ((SELECT COUNT(1)
                                 FROM "Notes" N
                                 WHERE 
                                     (
                                          (N."FormId" = FS."FormId" AND FS."PollingStationId" = N."PollingStationId") -- backwards compatibility
                                          OR N."SubmissionId" = FS."Id"
                                     )
                                   AND N."MonitoringObserverId" = fs."MonitoringObserverId") = 0 AND @hasNotes = false)
                            OR ((SELECT COUNT(1)
                                 FROM "Notes" N
                                 WHERE 
                                     (
                                          (N."FormId" = FS."FormId" AND FS."PollingStationId" = N."PollingStationId") -- backwards compatibility
                                          OR N."SubmissionId" = FS."Id"
                                      )
                                   AND N."MonitoringObserverId" = fs."MonitoringObserverId") > 0 AND @hasNotes = true))
                          AND (@fromDate is NULL OR FS."LastUpdatedAt" >= @fromDate::timestamp)
                          AND (@toDate is NULL OR FS."LastUpdatedAt" <= @toDate::timestamp)) c;
                  
                  WITH polling_station_submissions AS (SELECT psi."Id" AS                                     "SubmissionId",
                                                              'PSI'    AS                                     "FormType",
                                                              'PSI'    AS                                     "FormCode",
                                                              psif."Id"                                       "FormId",
                                                              psi."PollingStationId",
                                                              psi."MonitoringObserverId",
                                                              psi."Answers",
                                                              '[]'::jsonb AS "Attachments",
                                                              '[]'::jsonb AS "Notes",
                                                              psi."ArrivalTime",
                                                              psi."DepartureTime",
                                                              psi."Breaks",
                                                              psi."NumberOfQuestionsAnswered",
                                                              psi."NumberOfFlaggedAnswers",
                                                              0        AS                                     "MediaFilesCount",
                                                              0        AS                                     "NotesCount",
                                                              psi."LastUpdatedAt" AS "CreatedAt",
                                                              psi."LastUpdatedAt" AS "LastUpdatedAt",
                                                              psi."FollowUpStatus",
                                                              psi."IsCompleted"
                                                       FROM "PollingStationInformation" psi
                                                                INNER JOIN "PollingStationInformationForms" psif
                                                                           ON psif."Id" = psi."PollingStationInformationFormId"
                                                                INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource)  mo
                                                                           ON mo."MonitoringObserverId" = psi."MonitoringObserverId"
                                                       WHERE psi."ElectionRoundId" = @electionRoundId
                                                         AND (@COALITIONMEMBERID IS NULL OR mo."NgoId" = @COALITIONMEMBERID)
                                                         AND (@monitoringObserverId IS NULL OR mo."MonitoringObserverId" = @monitoringObserverId)
                                                         AND (@searchText IS NULL OR @searchText = ''
                                                           OR mo."DisplayName" ILIKE @searchText
                                                           OR mo."Email" ILIKE @searchText
                                                           OR mo."PhoneNumber" ILIKE @searchText
                                                           OR mo."MonitoringObserverId"::TEXT ILIKE @searchText)
                                                         AND (@monitoringObserverId IS NULL OR mo."MonitoringObserverId" = @monitoringObserverId)
                                                         AND (@monitoringObserverStatus IS NULL OR
                                                              mo."Status" = @monitoringObserverStatus)
                                                         AND (@formId IS NULL OR psi."PollingStationInformationFormId" = @formId)
                                                         AND (@fromDate is NULL OR
                                                              PSI."LastUpdatedAt" >= @fromDate::timestamp)
                                                         AND (@toDate is NULL OR
                                                              PSI."LastUpdatedAt" <= @toDate::timestamp)
                                                         AND (@questionsAnswered IS NULL
                                                           OR (@questionsAnswered = 'All' AND
                                                               psif."NumberOfQuestions" = psi."NumberOfQuestionsAnswered")
                                                           OR (@questionsAnswered = 'Some' AND
                                                               psif."NumberOfQuestions" <> psi."NumberOfQuestionsAnswered")
                                                           OR (@questionsAnswered = 'None' AND psi."NumberOfQuestionsAnswered" = 0))),
                       form_submissions AS (SELECT fs."Id"                                              AS "SubmissionId",
                                                   f."FormType",
                                                   f."Code"                                             AS "FormCode",
                                                   f."Id"                                               AS "FormId",
                                                   fs."PollingStationId",
                                                   fs."MonitoringObserverId",
                                                   fs."Answers",
                                                   COALESCE((select jsonb_agg(jsonb_build_object('QuestionId', "QuestionId", 'FileName', "FileName", 'MimeType', "MimeType", 'FilePath', "FilePath", 'UploadedFileName', "UploadedFileName", 'TimeSubmitted', "LastUpdatedAt"))
                                                             FROM "Attachments" a
                                                             WHERE
                                                                 (
                                                                     (A."FormId" = FS."FormId" AND FS."PollingStationId" = A."PollingStationId") -- backwards compatibility
                                                                         OR A."SubmissionId" = FS."Id"
                                                                     )
                                                               AND a."MonitoringObserverId" = fs."MonitoringObserverId"
                                                               AND a."IsDeleted" = false
                                                               AND a."IsCompleted" = true),'[]'::JSONB) AS "Attachments",
                                                   COALESCE((select jsonb_agg(jsonb_build_object('QuestionId', "QuestionId", 'Text', "Text", 'TimeSubmitted', "LastUpdatedAt"))
                                                             FROM "Notes" n
                                                             WHERE
                                                                 (
                                                                     (N."FormId" = FS."FormId" AND FS."PollingStationId" = N."PollingStationId") -- backwards compatibility
                                                                         OR N."SubmissionId" = FS."Id"
                                                                     )
                                                               AND n."MonitoringObserverId" = fs."MonitoringObserverId"), '[]'::JSONB) AS "Notes",
                                                   NULL::timestamp AS "ArrivalTime",
                                                   NULL::timestamp AS "DepartureTime",
                                                   '[]'::jsonb AS "Breaks",
                                                   fs."NumberOfQuestionsAnswered",
                                                   fs."NumberOfFlaggedAnswers",
                                                   (SELECT COUNT(1)
                                                    FROM "Attachments" A
                                                    WHERE a."MonitoringObserverId" = FS."MonitoringObserverId"
                                                    AND (
                                                          (A."FormId" = FS."FormId" AND FS."PollingStationId" = A."PollingStationId") -- backwards compatibility
                                                          OR A."SubmissionId" = FS."Id"
                                                      )
                                                      AND A."IsDeleted" = false
                                                      AND A."IsCompleted" = true)                       AS "MediaFilesCount",
                                                   (SELECT COUNT(1)
                                                    FROM "Notes" N
                                                    WHERE 
                                                        (
                                                          (N."FormId" = FS."FormId" AND FS."PollingStationId" = N."PollingStationId") -- backwards compatibility
                                                          OR N."SubmissionId" = FS."Id"
                                                        )
                                                      AND N."MonitoringObserverId" = fs."MonitoringObserverId") AS "NotesCount",
                                                   fs."CreatedAt"        AS "CreatedAt",
                                                   fs."LastUpdatedAt"        AS "LastUpdatedAt",
                                                   fs."FollowUpStatus",
                                                   fs."IsCompleted"
                                            FROM "FormSubmissions" fs
                                                     INNER JOIN "Forms" f ON f."Id" = fs."FormId"
                                                     INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource)  mo ON fs."MonitoringObserverId" = mo."MonitoringObserverId"
                                                     INNER JOIN "GetAvailableForms"(@electionRoundId, @ngoId, @dataSource)  AF ON AF."FormId" = fs."FormId"
                                            WHERE fs."ElectionRoundId" = @electionRoundId
                                              AND (@COALITIONMEMBERID IS NULL OR mo."NgoId" = @COALITIONMEMBERID)
                                              AND (@monitoringObserverId IS NULL OR mo."MonitoringObserverId" = @monitoringObserverId)
                                              AND (@searchText IS NULL OR @searchText = ''
                                                OR mo."DisplayName" ILIKE @searchText
                                                OR mo."Email" ILIKE @searchText
                                                OR mo."PhoneNumber" ILIKE @searchText
                                                OR mo."MonitoringObserverId"::TEXT ILIKE @searchText)
                                              AND (@monitoringObserverId IS NULL OR mo."MonitoringObserverId" = @monitoringObserverId)
                                              AND (@monitoringObserverStatus IS NULL OR mo."Status" = @monitoringObserverStatus)
                                              AND (@formId IS NULL OR fs."FormId" = @formId)
                                              AND (@fromDate is NULL OR
                                                   FS."LastUpdatedAt" >= @fromDate::timestamp)
                                              AND (@toDate is NULL OR FS."LastUpdatedAt" <= @toDate::timestamp)
                                              AND (@questionsAnswered IS NULL
                                                OR (@questionsAnswered = 'All' AND f."NumberOfQuestions" = fs."NumberOfQuestionsAnswered")
                                                OR (@questionsAnswered = 'Some' AND
                                                    f."NumberOfQuestions" <> fs."NumberOfQuestionsAnswered")
                                                OR (@questionsAnswered = 'None' AND fs."NumberOfQuestionsAnswered" = 0)))
                  SELECT s."SubmissionId",
                         s."CreatedAt",
                         s."LastUpdatedAt",
                         s."FormId",
                         s."FormCode",
                         s."FormType",
                         ps."Id"          AS "PollingStationId",
                         ps."Level1",
                         ps."Level2",
                         ps."Level3",
                         ps."Level4",
                         ps."Level5",
                         ps."Number",
                         s."MonitoringObserverId",
                         mo."DisplayName" AS "ObserverName",
                         mo."Email",
                         mo."PhoneNumber",
                         mo."IsOwnObserver",
                         mo."Status",
                         mo."Tags",
                         MO."NgoName",
                         s."NumberOfQuestionsAnswered",
                         s."NumberOfFlaggedAnswers",
                         s."MediaFilesCount",
                         s."NotesCount",
                         s."Answers",
                         s."Notes",
                         s."Attachments",
                         s."ArrivalTime",
                         s."DepartureTime",
                         s."Breaks",
                         s."FollowUpStatus",
                         s."IsCompleted",
                         mo."Status"         "MonitoringObserverStatus"
                  FROM (SELECT *
                        FROM polling_station_submissions
                        UNION ALL
                        SELECT *
                        FROM form_submissions) s
                           INNER JOIN "PollingStations" ps ON ps."Id" = s."PollingStationId"
                           INNER JOIN "GetAvailableMonitoringObservers"(@electionRoundId, @ngoId, @dataSource) mo ON mo."MonitoringObserverId" = s."MonitoringObserverId"
                  WHERE (@formType IS NULL OR s."FormType" = @formType)
                    AND (@level1 IS NULL OR ps."Level1" = @level1)
                    AND (@level2 IS NULL OR ps."Level2" = @level2)
                    AND (@level3 IS NULL OR ps."Level3" = @level3)
                    AND (@level4 IS NULL OR ps."Level4" = @level4)
                    AND (@level5 IS NULL OR ps."Level5" = @level5)
                    AND (@pollingStationNumber IS NULL OR ps."Number" = @pollingStationNumber)
                    AND (@hasFlaggedAnswers IS NULL
                      OR (s."NumberOfFlaggedAnswers" = 0 AND @hasFlaggedAnswers = false)
                      OR (s."NumberOfFlaggedAnswers" > 0 AND @hasFlaggedAnswers = true))
                    AND (@followUpStatus IS NULL OR s."FollowUpStatus" = @followUpStatus)
                    AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR mo."Tags" && @tagsFilter)
                    AND (@hasNotes IS NULL
                      OR (s."NotesCount" = 0 AND @hasNotes = false)
                      OR (s."NotesCount" > 0 AND @hasNotes = true))
                    AND (@hasAttachments IS NULL
                      OR (s."MediaFilesCount" = 0 AND @hasAttachments = false)
                      OR (s."MediaFilesCount" > 0 AND @hasAttachments = true))
                  ORDER BY CASE WHEN @sortExpression = 'LastUpdatedAt ASC' THEN s."LastUpdatedAt" END ASC,
                           CASE WHEN @sortExpression = 'LastUpdatedAt DESC' THEN s."LastUpdatedAt" END DESC,
                           CASE WHEN @sortExpression = 'CreatedAt ASC' THEN s."CreatedAt" END ASC,
                            CASE WHEN @sortExpression = 'CreatedAt DESC' THEN s."CreatedAt" END DESC,
                           CASE WHEN @sortExpression = 'FormCode ASC' THEN s."FormCode" END ASC,
                           CASE WHEN @sortExpression = 'FormCode DESC' THEN s."FormCode" END DESC,
                           CASE WHEN @sortExpression = 'FormType ASC' THEN s."FormType" END ASC,
                           CASE WHEN @sortExpression = 'FormType DESC' THEN s."FormType" END DESC,
                           CASE WHEN @sortExpression = 'Level1 ASC' THEN ps."Level1" END ASC,
                           CASE WHEN @sortExpression = 'Level1 DESC' THEN ps."Level1" END DESC,
                           CASE WHEN @sortExpression = 'Level2 ASC' THEN ps."Level2" END ASC,
                           CASE WHEN @sortExpression = 'Level2 DESC' THEN ps."Level2" END DESC,
                           CASE WHEN @sortExpression = 'Level3 ASC' THEN ps."Level3" END ASC,
                           CASE WHEN @sortExpression = 'Level3 DESC' THEN ps."Level3" END DESC,
                           CASE WHEN @sortExpression = 'Level4 ASC' THEN ps."Level4" END ASC,
                           CASE WHEN @sortExpression = 'Level4 DESC' THEN ps."Level4" END DESC,
                           CASE WHEN @sortExpression = 'Level5 ASC' THEN ps."Level5" END ASC,
                           CASE WHEN @sortExpression = 'Level5 DESC' THEN ps."Level5" END DESC,
                           CASE WHEN @sortExpression = 'Number ASC' THEN ps."Number" END ASC,
                           CASE WHEN @sortExpression = 'Number DESC' THEN ps."Number" END DESC,
                           CASE WHEN @sortExpression = 'ObserverName ASC' THEN mo."DisplayName" END ASC,
                           CASE WHEN @sortExpression = 'ObserverName DESC' THEN mo."DisplayName" END DESC,
                           CASE WHEN @sortExpression = 'NumberOfFlaggedAnswers ASC' THEN s."NumberOfFlaggedAnswers" END ASC,
                           CASE WHEN @sortExpression = 'NumberOfFlaggedAnswers DESC' THEN s."NumberOfFlaggedAnswers" END DESC,
                           CASE WHEN @sortExpression = 'NumberOfQuestionsAnswered ASC' THEN s."NumberOfQuestionsAnswered" END ASC,
                           CASE WHEN @sortExpression = 'NumberOfQuestionsAnswered DESC' THEN s."NumberOfQuestionsAnswered" END DESC,
                           CASE WHEN @sortExpression = 'MediaFilesCount ASC' THEN s."MediaFilesCount" END ASC,
                           CASE WHEN @sortExpression = 'MediaFilesCount DESC' THEN s."MediaFilesCount" END DESC,
                           CASE WHEN @sortExpression = 'NotesCount ASC' THEN s."NotesCount" END ASC,
                           CASE WHEN @sortExpression = 'NotesCount DESC' THEN s."NotesCount" END DESC,
                           CASE WHEN @sortExpression = 'MonitoringObserverStatus ASC' THEN mo."Status" END ASC,
                           CASE WHEN @sortExpression = 'MonitoringObserverStatus DESC' THEN mo."Status" END DESC
                  OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            coalitionMemberId = req.CoalitionMemberId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            monitoringObserverId = req.MonitoringObserverId,
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            formType = req.FormTypeFilter?.ToString(),
            level1 = req.Level1Filter,
            level2 = req.Level2Filter,
            level3 = req.Level3Filter,
            level4 = req.Level4Filter,
            level5 = req.Level5Filter,
            pollingStationNumber = req.PollingStationNumberFilter,
            hasFlaggedAnswers = req.HasFlaggedAnswers,
            followUpStatus = req.FollowUpStatus?.ToString(),
            tagsFilter = req.TagsFilter ?? [],
            monitoringObserverStatus = req.MonitoringObserverStatus?.ToString(),
            formId = req.FormId,
            hasNotes = req.HasNotes,
            hasAttachments = req.HasAttachments,
            questionsAnswered = req.QuestionsAnswered?.ToString(),
            fromDate = req.FromDateFilter?.ToString("O"),
            toDate = req.ToDateFilter?.ToString("O"),
            dataSource = req.DataSource?.ToString(),
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        int totalRowCount;
        List<DetailedSubmissionEntry> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<DetailedSubmissionEntry>().ToList();
        }

        entries = (await Task.WhenAll(
            entries.Select(async entry => entry with
            {
                Attachments = await Task.WhenAll(
                    entry.Attachments.Select(async attachment =>
                    {
                        var result =
                            await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);
                        return result is GetPresignedUrlResult.Ok(var url, _, var urlValidityInSeconds)
                            ? attachment with { PresignedUrl = url, UrlValidityInSeconds = urlValidityInSeconds }
                            : attachment;
                    })
                )
            })
        )).ToList();

        return TypedResults.Ok(
            new PagedResponse<DetailedSubmissionEntry>(entries, totalRowCount, req.PageNumber, req.PageSize));
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(DetailedSubmissionEntry.CreatedAt)} DESC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, "FormCode",
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"FormCode {sortOrder}";
        }

        if (string.Equals(sortColumnName, "FormType",
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"FormType {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.Level1),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(DetailedSubmissionEntry.Level1)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.Level2),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(DetailedSubmissionEntry.Level2)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.Level3),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(DetailedSubmissionEntry.Level3)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.Level4),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(DetailedSubmissionEntry.Level4)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.Level5),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(DetailedSubmissionEntry.Level5)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.Number),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(DetailedSubmissionEntry.Number)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.ObserverName),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(DetailedSubmissionEntry.ObserverName)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.Email),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(DetailedSubmissionEntry.Email)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.PhoneNumber),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(DetailedSubmissionEntry.PhoneNumber)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.Tags),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(DetailedSubmissionEntry.Tags)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.NumberOfQuestionsAnswered),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(DetailedSubmissionEntry.NumberOfQuestionsAnswered)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(DetailedSubmissionEntry.NumberOfFlaggedAnswers)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, "MediaFilesCount",
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"MediaFilesCount {sortOrder}";
        }

        if (string.Equals(sortColumnName, "NotesCount",
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"NotesCount {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.CreatedAt),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(DetailedSubmissionEntry.CreatedAt)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(DetailedSubmissionEntry.LastUpdatedAt),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(DetailedSubmissionEntry.LastUpdatedAt)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, "MonitoringObserverStatus",
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"MonitoringObserverStatus {sortOrder}";
        }

        return $"{nameof(DetailedSubmissionEntry.CreatedAt)} DESC";
    }
}
