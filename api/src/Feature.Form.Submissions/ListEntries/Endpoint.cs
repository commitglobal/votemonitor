using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Form.Submissions.ListEntries;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, PagedResponse<FormSubmissionEntry>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:byEntry");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(x => { x.Summary = "Lists form submissions by entry in our system"; });
    }

    public override async Task<PagedResponse<FormSubmissionEntry>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = """
                  SELECT SUM(count)
                  FROM
                      (SELECT count(*) AS count
                       FROM "PollingStationInformation" psi
                       INNER JOIN "PollingStationInformationForms" psif ON psif."Id" = psi."PollingStationInformationFormId"
                       INNER JOIN "PollingStations" ps ON ps."Id" = psi."PollingStationId"
                       INNER JOIN "MonitoringObservers" mo ON mo."Id" = psi."MonitoringObserverId"
                       INNER JOIN "MonitoringNgos" mn ON mn."Id" = mo."MonitoringNgoId"
                       INNER JOIN "Observers" o ON o."Id" = mo."ObserverId"
                       INNER JOIN "AspNetUsers" u ON u."Id" = o."ApplicationUserId"
                       WHERE mn."ElectionRoundId" = @electionRoundId
                           AND mn."NgoId" = @ngoId
                           AND (@monitoringObserverId IS NULL OR mo."Id" = @monitoringObserverId)
                           AND (@searchText IS NULL OR @searchText = '' OR u."FirstName" ILIKE @searchText OR u."LastName" ILIKE @searchText OR u."Email" ILIKE @searchText OR u."PhoneNumber" ILIKE @searchText)
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
                       UNION ALL SELECT count(*) AS count
                       FROM "FormSubmissions" fs
                       INNER JOIN "Forms" f ON f."Id" = fs."FormId"
                       INNER JOIN "PollingStations" ps ON ps."Id" = fs."PollingStationId"
                       INNER JOIN "MonitoringObservers" mo ON mo."Id" = fs."MonitoringObserverId"
                       INNER JOIN "MonitoringNgos" mn ON mn."Id" = mo."MonitoringNgoId"
                       INNER JOIN "Observers" o ON o."Id" = mo."ObserverId"
                       INNER JOIN "AspNetUsers" u ON u."Id" = o."ApplicationUserId"
                       WHERE mn."ElectionRoundId" = @electionRoundId
                           AND mn."NgoId" = @ngoId
                           AND (@monitoringObserverId IS NULL OR mo."Id" = @monitoringObserverId)
                           AND (@searchText IS NULL OR @searchText = '' OR u."FirstName" ILIKE @searchText OR u."LastName" ILIKE @searchText OR u."Email" ILIKE @searchText OR u."PhoneNumber" ILIKE @searchText)
                           AND (@formType IS NULL OR f."FormType" = @formType)
                           AND (@level1 IS NULL OR ps."Level1" = @level1)
                           AND (@level2 IS NULL OR ps."Level2" = @level2)
                           AND (@level3 IS NULL OR ps."Level3" = @level3)
                           AND (@level4 IS NULL OR ps."Level4" = @level4)
                           AND (@level5 IS NULL OR ps."Level5" = @level5)
                           AND (@pollingStationNumber IS NULL OR ps."Number" = @pollingStationNumber)
                           AND (@hasFlaggedAnswers is NULL OR (fs."NumberOfFlaggedAnswers" = 0 AND @hasFlaggedAnswers = false) OR ("NumberOfFlaggedAnswers" > 0 AND @hasFlaggedAnswers = true))
                           AND (@followUpStatus is NULL OR fs."FollowUpStatus" = @followUpStatus)
                           AND (@tagsFilter IS NULL OR cardinality(@tagsFilter) = 0 OR mo."Tags" && @tagsFilter)
                           AND (@monitoringObserverStatus IS NULL OR mo."Status" = @monitoringObserverStatus)
                           AND (@formId IS NULL OR fs."FormId" = @formId)
                           AND (@questionsAnswered is null 
                                       OR (@questionsAnswered = 'All' AND f."NumberOfQuestions" = fs."NumberOfQuestionsAnswered")
                                       OR (@questionsAnswered = 'Some' AND f."NumberOfQuestions" <> fs."NumberOfQuestionsAnswered") 
                                       OR (@questionsAnswered = 'None' AND fs."NumberOfQuestionsAnswered" = 0))
                           AND (@hasAttachments is NULL
                                OR ((SELECT COUNT(1) FROM "Attachments" WHERE "FormId" = fs."FormId" AND "MonitoringObserverId" = fs."MonitoringObserverId" AND fs."PollingStationId" = "PollingStationId" AND "IsDeleted" = false AND "IsCompleted" = true) = 0 AND @hasAttachments = false) 
                                OR ((SELECT COUNT(1) FROM "Attachments" WHERE "FormId" = fs."FormId" AND "MonitoringObserverId" = fs."MonitoringObserverId" AND fs."PollingStationId" = "PollingStationId" AND "IsDeleted" = false AND "IsCompleted" = true) > 0 AND @hasAttachments = true))
                           AND (@hasNotes is NULL 
                                OR ((SELECT COUNT(1) FROM "Notes" WHERE "FormId" = fs."FormId" AND "MonitoringObserverId" = fs."MonitoringObserverId" AND  fs."PollingStationId" = "PollingStationId") = 0 AND @hasNotes = false) 
                                OR ((SELECT COUNT(1) FROM "Notes" WHERE "FormId" = fs."FormId" AND "MonitoringObserverId" = fs."MonitoringObserverId" AND  fs."PollingStationId" = "PollingStationId") > 0 AND @hasNotes = true))
                  ) c;

                  WITH polling_station_submissions AS (
                      SELECT psi."Id" AS "SubmissionId",
                             'PSI' AS "FormType",
                             'PSI' AS "FormCode",
                             psi."PollingStationId",
                             psi."MonitoringObserverId",
                             psi."NumberOfQuestionsAnswered",
                             psi."NumberOfFlaggedAnswers",
                             0 AS "MediaFilesCount",
                             0 AS "NotesCount",
                             COALESCE(psi."LastModifiedOn", psi."CreatedOn") "TimeSubmitted",
                             psi."FollowUpStatus"
                      FROM "PollingStationInformation" psi
                      INNER JOIN "PollingStationInformationForms" psif ON psif."Id" = psi."PollingStationInformationFormId"
                      INNER JOIN "MonitoringObservers" mo ON mo."Id" = psi."MonitoringObserverId"
                      INNER JOIN "MonitoringNgos" mn ON mn."Id" = mo."MonitoringNgoId"
                      WHERE mn."ElectionRoundId" = @electionRoundId
                        AND mn."NgoId" = @ngoId
                        AND (@monitoringObserverId IS NULL OR mo."Id" = @monitoringObserverId)
                        AND (@monitoringObserverStatus IS NULL OR mo."Status" = @monitoringObserverStatus)
                        AND (@formId IS NULL OR psi."PollingStationInformationFormId" = @formId)
                        AND (@questionsAnswered IS NULL 
                             OR (@questionsAnswered = 'All' AND psif."NumberOfQuestions" = psi."NumberOfQuestionsAnswered")
                             OR (@questionsAnswered = 'Some' AND psif."NumberOfQuestions" <> psi."NumberOfQuestionsAnswered")
                             OR (@questionsAnswered = 'None' AND psi."NumberOfQuestionsAnswered" = 0))
                  ),
                  form_submissions AS (
                      SELECT fs."Id" AS "SubmissionId",
                             f."FormType",
                             f."Code" AS "FormCode",
                             fs."PollingStationId",
                             fs."MonitoringObserverId",
                             fs."NumberOfQuestionsAnswered",
                             fs."NumberOfFlaggedAnswers",
                             (
                                 SELECT COUNT(1)
                                 FROM "Attachments"
                                 WHERE "FormId" = fs."FormId"
                                   AND "MonitoringObserverId" = fs."MonitoringObserverId"
                                   AND fs."PollingStationId" = "PollingStationId"
                                   AND "IsDeleted" = false AND "IsCompleted" = true
                             ) AS "MediaFilesCount",
                             (
                                 SELECT COUNT(1)
                                 FROM "Notes"
                                 WHERE "FormId" = fs."FormId"
                                   AND "MonitoringObserverId" = fs."MonitoringObserverId"
                                   AND fs."PollingStationId" = "PollingStationId"
                             ) AS "NotesCount",
                             COALESCE(fs."LastModifiedOn", fs."CreatedOn") AS "TimeSubmitted",
                             fs."FollowUpStatus"
                      FROM "FormSubmissions" fs
                      INNER JOIN "Forms" f ON f."Id" = fs."FormId"
                      INNER JOIN "MonitoringObservers" mo ON fs."MonitoringObserverId" = mo."Id"
                      INNER JOIN "MonitoringNgos" mn ON mn."Id" = mo."MonitoringNgoId"
                      WHERE mn."ElectionRoundId" = @electionRoundId
                        AND mn."NgoId" = @ngoId
                        AND (@monitoringObserverId IS NULL OR mo."Id" = @monitoringObserverId)
                        AND (@monitoringObserverStatus IS NULL OR mo."Status" = @monitoringObserverStatus)
                        AND (@formId IS NULL OR fs."FormId" = @formId)
                        AND (@questionsAnswered IS NULL 
                             OR (@questionsAnswered = 'All' AND f."NumberOfQuestions" = fs."NumberOfQuestionsAnswered")
                             OR (@questionsAnswered = 'Some' AND f."NumberOfQuestions" <> fs."NumberOfQuestionsAnswered")
                             OR (@questionsAnswered = 'None' AND fs."NumberOfQuestionsAnswered" = 0))
                  )
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
                         u."FirstName" || ' ' || u."LastName" AS "ObserverName",
                         u."Email",
                         u."PhoneNumber",
                         mo."Status",
                         mo."Tags",
                         s."NumberOfQuestionsAnswered",
                         s."NumberOfFlaggedAnswers",
                         s."MediaFilesCount",
                         s."NotesCount",
                         s."FollowUpStatus"
                  FROM (
                      SELECT * FROM polling_station_submissions
                      UNION ALL
                      SELECT * FROM form_submissions
                  ) s
                  INNER JOIN "PollingStations" ps ON ps."Id" = s."PollingStationId"
                  INNER JOIN "MonitoringObservers" mo ON mo."Id" = s."MonitoringObserverId"
                  INNER JOIN "MonitoringNgos" mn ON mn."Id" = mo."MonitoringNgoId"
                  INNER JOIN "Observers" o ON o."Id" = mo."ObserverId"
                  INNER JOIN "AspNetUsers" u ON u."Id" = o."ApplicationUserId"
                  WHERE mn."ElectionRoundId" = @electionRoundId
                    AND mn."NgoId" = @ngoId
                    AND (@monitoringObserverId IS NULL OR mo."Id" = @monitoringObserverId)
                    AND (@searchText IS NULL OR @searchText = '' 
                         OR u."FirstName" ILIKE @searchText 
                         OR u."LastName" ILIKE @searchText 
                         OR u."Email" ILIKE @searchText 
                         OR u."PhoneNumber" ILIKE @searchText)
                    AND (@formType IS NULL OR s."FormType" = @formType)
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
                  ORDER BY
                      CASE WHEN @sortExpression = 'TimeSubmitted ASC' THEN s."TimeSubmitted" END ASC,
                      CASE WHEN @sortExpression = 'TimeSubmitted DESC' THEN s."TimeSubmitted" END DESC,
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
                      CASE WHEN @sortExpression = 'ObserverName ASC' THEN u."FirstName" || ' ' || u."LastName" END ASC,
                      CASE WHEN @sortExpression = 'ObserverName DESC' THEN u."FirstName" || ' ' || u."LastName" END DESC
                  OFFSET @offset ROWS
                  FETCH NEXT @pageSize ROWS ONLY;
                  
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
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
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting),
        };

        int totalRowCount;
        List<FormSubmissionEntry> entries;

        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            totalRowCount = multi.Read<int>().Single();
            entries = multi.Read<FormSubmissionEntry>().ToList();
        }

        return new PagedResponse<FormSubmissionEntry>(entries, totalRowCount, req.PageNumber, req.PageSize);
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(FormSubmissionEntry.TimeSubmitted)} DESC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.FormCode),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.FormCode)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.FormType),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.FormType)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level1),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Level1)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level2),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Level2)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level3),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Level3)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level4),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Level4)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level5),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Level5)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Number),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Number)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.ObserverName),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.ObserverName)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Email),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Email)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.PhoneNumber),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.PhoneNumber)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Tags),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Tags)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.NumberOfQuestionsAnswered),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.NumberOfQuestionsAnswered)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.NumberOfFlaggedAnswers),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.NumberOfFlaggedAnswers)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.MediaFilesCount),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.MediaFilesCount)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.NotesCount),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.NotesCount)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.TimeSubmitted),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.TimeSubmitted)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.MonitoringObserverStatus),
                StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.MonitoringObserverStatus)} {sortOrder}";
        }

        return $"{nameof(FormSubmissionEntry.TimeSubmitted)} DESC";
    }
}